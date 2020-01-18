using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed; //speed to spin

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, spinSpeed)), Time.deltaTime * spinSpeed);
    }
}
