using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMover : MonoBehaviour
{
    public float speed; //speed multiplier
    public float spinSpeed; //ball spin multiplier
    public float maxSpeed; //maximum speed of the player
    public Transform camBase; //camera base to transform movement around
    public Rigidbody rb; //ball's rigidbody
    Vector3 direction; //direction of movement
    public float normalMaxSpinSpeed; //normal maximum spin speed of the rigidbody

    //dash values
    bool dashEnabled; //is dashing enabled
    public float dashTimeLimit; //how long dashes last
    float dashAdjust; //speed adjust for dash
    public float dashIncrease; //increased speed multiplier for dashing

    // Start is called before the first frame update
    void Start()
    {
        dashAdjust = 0f;
        dashEnabled = false;
        direction = Vector3.zero;
        rb.maxAngularVelocity = normalMaxSpinSpeed;
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

        //apply force
        Vector3 moveDirec = direction.x * right + direction.z * forward;
        Vector3 spinDirec = direction.z * right - direction.x * forward;

        if (!dashEnabled)
        {
            rb.AddForce(moveDirec * speed, ForceMode.Acceleration);
            rb.AddTorque(spinDirec * spinSpeed, ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(moveDirec * speed * (1 + dashIncrease * dashAdjust), ForceMode.Acceleration);
            rb.AddTorque(spinDirec * spinSpeed * (1 + dashIncrease*dashAdjust), ForceMode.VelocityChange);
        }

        //limit lateral speed if not dashing
        if(!dashEnabled && rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    //Enables dashing
    public void EnableDash(float dashLevel)
    {
        //called by string to enable restarting
        StopCoroutine("DashLoop");
        dashAdjust = dashLevel;
        StartCoroutine("DashLoop");
    }

    //enables dashing
    public IEnumerator DashLoop()
    {
        Pickup p = GetComponent<Pickup>();
        dashEnabled = true;
        p.isDashing = true;
        LevelManager.Instance.StartDashAnim();
        yield return new WaitForSeconds(dashTimeLimit);
        dashEnabled = false;
        rb.maxAngularVelocity = normalMaxSpinSpeed;
        p.SetIrregulars(true);
        p.isDashing = false;
        LevelManager.Instance.StopDashAnim();
    }
}
