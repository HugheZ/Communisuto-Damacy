using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float sensitivity; //sensitivity of look movement
    public float lerpSensitivity; //sensitivity of movement
    public float distance; //distance from the player
    public float distanceAdd; //added distance to prevent wobble
    public float hitYOffset; //offset for y on cam hit
    public Transform player; //player position ref
    public Transform cam; //camera's transform
    public float yMax; //max y rot
    public float yMin; //min y rot
    float yAngle; //current y angle
    public LayerMask notPlayerLayers; //layers that aren't the player

    // Start is called before the first frame update
    void Start()
    {
        yAngle = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //set angle by mouse
        float xAngle = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        yAngle += Input.GetAxis("Mouse Y") * sensitivity;

        yAngle = Mathf.Clamp(yAngle, yMin, yMax);

        transform.localEulerAngles = new Vector3(-yAngle, xAngle, 0);

        //set position by transform
        //first raycast to see if anything between player and cam
        RaycastHit hit;
        Physics.Raycast(player.position, -1 * transform.forward, out hit, distance + distanceAdd, notPlayerLayers);

        //if hits something, move cam to that distance, else reset to standard position
        if (hit.collider)
        {
            Vector3 pos = transform.InverseTransformPoint(hit.point);
            pos.x = 0;
            pos.y = hitYOffset;
            cam.localPosition = pos;
        }
        else
        {
            cam.localPosition = new Vector3(0, 0, -distance);
        }
        Vector3 velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, player.position, ref velocity, Time.deltaTime * lerpSensitivity);
    }
}
