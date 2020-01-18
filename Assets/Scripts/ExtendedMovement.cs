using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedMovement : MonoBehaviour
{
    public BallMover movement; //base movement
    public Pickup ballData; //data for ball size
    public Transform camBase; //base of camera for reletive movement
    public float dashChargeRate; //rate of dash charging
    Rigidbody rb; //rigidbody
    public LayerMask genericLayer; //layer on which everything sits
    public bool grounded; //whether the player is grounded
    public float chargingSpinSpeed; //speed to spin when charging
    public float maxSpinSpeed; //maximum spin speed of rigidbody
    public float maxVelocityToSmoke; //max velocity needed to be hit in order to start smoking
    public float dragOnBreak; //drag to apply when breaking
    float normalDrag;
    public ParticleSystem smoke; //smoke particles

    //audio values
    public AudioSource chargeSource; //source of charging sound
    public AudioSource sound; //source of regular sound
    public AudioClip hop; //hop sound
    public AudioClip dash; //dash sound
    public AudioClip screech; //screeching sound

    //control bools
    public float dashChargeAmount; //how far the player has charged a dash
    bool chargingDash; //is the player charging a dash
    bool dashFlag; //player has dashed
    bool jumped; //has the player jumped
    bool breaking; //breaking or not

    //movement values
    public float jumpForceFactor; //factor to apply jump force, multiplied to movement speed
    public float dashForceFactor; //factor to apply dash force, multiplied to movement speed


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalDrag = rb.drag;

        dashChargeAmount = 0f;
        chargingDash = false;
        dashFlag = false;
        jumped = false;
        breaking = false;
    }

    //determines if the ball is grounded by raycast downwards, checking if the ball is on the ground or an object too big to pickup
    bool IsGrounded()
    {
        Collider[] hit;
        //Physics.Raycast(transform.position, Vector3.down, out hit, ballData.sphere.radius + .03f, genericLayer);
        hit = Physics.OverlapSphere(transform.position + Vector3.down * .01f, ballData.sphere.radius, genericLayer);
        //if there was something below the ball such that the ball's main collider is touching it, grounded
        if (hit.Length > 0)
        {
            return true;
        }
        //else not grounded
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        //if grounded, allow for special movement
        if (grounded) {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //play sound if not yet
                //pressing shift key, enable dash sound
                if (!chargeSource.isPlaying) chargeSource.Play();

                if (!chargingDash) GetComponent<Pickup>().SetIrregulars(false);
                //increment dash
                chargingDash = true;
                dashChargeAmount += dashChargeRate * Time.deltaTime;
                dashChargeAmount = Mathf.Clamp(dashChargeAmount, 0f, 1f);

                //increase charging sound pitch
                chargeSource.pitch = Mathf.Lerp(1, 1.5f, dashChargeAmount);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                //charge done, go through
                chargingDash = false;
                dashFlag = true;

                //stop charging source
                if (chargeSource.isPlaying) chargeSource.Stop();
            }
            //else if breaking
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                breaking = true;
            }
            //if jump pressed and grounded, jump (last to avoid charge jumping)
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                jumped = true;

                //reset charge value
                chargingDash = false;
                dashChargeAmount = 0f;
            }
            else
            {
                breaking = false;
            }
        }

        //disable/enable movement
        if (grounded)
        {
            if (chargingDash)
            {
                movement.enabled = false;
                rb.constraints = RigidbodyConstraints.FreezePosition;
            }
            else
            {
                movement.enabled = true;
                rb.constraints = RigidbodyConstraints.None;
            }
        }
        else
        {
            movement.enabled = true;
            breaking = false;
            rb.constraints = RigidbodyConstraints.None;

            //if playing charge source, stop it
            if (chargeSource.isPlaying) chargeSource.Stop();
        }

        //if breaking, change speed and see if we can smoke
        if (breaking)
        {
            //apply breakd rag
            rb.drag = dragOnBreak;

            //see if we need to play smoke and sound
            if (rb.velocity.magnitude >= maxVelocityToSmoke && !smoke.isPlaying)
            {
                //smoke.Play();
                sound.PlayOneShot(screech);
            }
            //else if (smoke.isPlaying) smoke.Stop();
        }
        else
        {
            rb.drag = normalDrag;
            //if (smoke.isPlaying) smoke.Stop();
        }

        //control smoke
        if (chargingDash || (breaking && rb.velocity.magnitude >= maxVelocityToSmoke))
        {
            if (!smoke.isPlaying) smoke.Play();
        }
        else
        {
            if (smoke.isPlaying) smoke.Stop();
        }
    }

    //fixed update for physics calculations
    private void FixedUpdate()
    {
        //check groundedness
        grounded = IsGrounded();

        //check special movements
        //if jumped, jump
        if (jumped)
        {
            jumped = false;
            rb.AddForce(Vector3.up * jumpForceFactor, ForceMode.Impulse);
            
            //play sound
            sound.PlayOneShot(hop);
        }
        //if dashed, dash
        if (dashFlag)
        {
            dashFlag = false;
            movement.EnableDash(dashChargeAmount);
            dashChargeAmount = 0f;

            //play dash sound
            sound.PlayOneShot(dash);
        }
        else if (chargingDash) //else if charging, spin the ball quickly
        {
            rb.maxAngularVelocity = maxSpinSpeed;
            rb.angularVelocity = camBase.right * chargingSpinSpeed;
        }
    }
}
