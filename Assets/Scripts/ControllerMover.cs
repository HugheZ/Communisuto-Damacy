using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMover : MonoBehaviour
{
    public float speed; //speed multiplier
    public float spinSpeed; //ball spin multiplier
    public float gravFactor; //factor to pull ball down by
    public float maxSpeed; //maximum speed of the player
    public Transform camBase; //camera base to transform movement around
    public Rigidbody rb; //ball's rigidbody
    public CharacterController cc; //controller for the character
    Vector3 direction; //direction of movement

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");
    }

    // Fixed update movement, better for physics
    void FixedUpdate()
    {
        //save y and apply roation change

        //Adapted from: https://forum.unity.com/threads/moving-character-relative-to-camera.383086/
        Vector3 forward = camBase.forward;
        Vector3 right = camBase.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        //End adaptation

        //gravity
        if (!cc.isGrounded)
        {
            direction.y += gravFactor;
        }
        else
        {
            direction.y = -1;
        }

        //apply force
        Vector3 moveDirec = speed * (direction.x * right + direction.z * forward);
        Vector3 spinDirec = direction.z * right - direction.x * forward;
        cc.Move(moveDirec * Time.fixedDeltaTime);
        rb.AddTorque(spinDirec * speed, ForceMode.VelocityChange);

        transform.position = new Vector3(transform.position.x, transform.TransformPoint(rb.transform.position).y, transform.position.z);

        //limit lateral speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
