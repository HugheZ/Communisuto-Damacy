using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public GameObject player;
    public float offsetDist;
    public float dampFactor;
    Vector3 offset;
    public float rotateSpeed;
    public float rotateAmount;
    public float rotationAngle = 0;

    void Start()
    {
        offset = new Vector3(0, offsetDist * .5f, -offsetDist);
        transform.position = player.transform.position + offset;
        transform.LookAt(player.transform);
    }

    void FixedUpdate()
    {
        if (Input.GetKey("q"))
        {
            rotationAngle += rotateSpeed;
        }
        else if (Input.GetKey("e"))
        {
            rotationAngle -= rotateSpeed;
        }

        Vector3 rotOffset = Quaternion.AngleAxis(rotationAngle*Mathf.Rad2Deg, Vector3.up)*offset;
        Vector3 newPos = player.transform.position + rotOffset;
        Vector3 zero = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref zero, dampFactor);
        transform.LookAt(player.transform);
    }
}
