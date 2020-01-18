using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    Vector3 direction;
    CharacterController cc;
    public float speed;
    public float jumpForce;
    CameraScript cs;
    public float gravity;

    //bool controllers
    public bool jump;

	// Use this for initialization
	void Start () {
        cs = Camera.main.GetComponent<CameraScript>();
        cc = GetComponent<CharacterController>();
        direction = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {
        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");

        if(cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            direction.y = jumpForce;
            jump = true;
        }
	}

    private void FixedUpdate()
    {
        //float cos = Mathf.Cos(cs.rotationAngle);
        //float sin = Mathf.Sin(cs.rotationAngle);
        float scale = Time.fixedDeltaTime * speed;

        //jumping
        if (cc.isGrounded && direction.y <= 0)
        {
            jump = false;
            direction.y = 0;
        }
        direction.y += gravity;

        cc.Move(transform.rotation * new Vector3(direction.x * scale, direction.y * Time.fixedDeltaTime, direction.z * scale));

        //cc.Move(Quaternion.AngleAxis(Mathf.Rad2Deg * cs.rotationAngle, Vector3.up) * new Vector3(direction.x * scale, direction.y * Time.fixedDeltaTime, direction.z*scale));
        //cc.Move(new Vector3((direction.z * sin + direction.x * cos)*speed, rb.velocity.y, (direction.z * cos - direction.x * sin)*speed));
    }
}
