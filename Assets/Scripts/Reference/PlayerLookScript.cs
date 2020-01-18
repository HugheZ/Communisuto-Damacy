using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookScript : MonoBehaviour {

    public float sensitivity;
    public GameObject head;
    public float maxY;
    public float minY;
    float yAngle;
    Camera cam;
    public Texture reticle;

	// Use this for initialization
	void Start () {
        yAngle = 0;
        cam = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
        float xAngle = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        yAngle += Input.GetAxis("Mouse Y") * sensitivity;

        yAngle = Mathf.Clamp(yAngle, minY, maxY);

        head.transform.localEulerAngles = new Vector3(-yAngle, 0, 0);
        transform.localEulerAngles = new Vector3(0, xAngle, 0);
	}

    private void OnGUI()
    {
        int size = 30;
        float posX = cam.pixelWidth / 2 - size / 4;
        float posY = cam.pixelHeight / 2 - size / 2;
        GUI.Label(new Rect(posX, posY, size, size), reticle);
    }
}
