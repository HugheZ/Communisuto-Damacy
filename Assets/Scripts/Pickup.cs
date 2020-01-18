using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    const float GROWTH_RATE = .01f; //rate to grow radius on pickup
    const float CAM_OFFSET_FACTOR = 1.7f; //offset for camera on pickup

    public int size; //size of the player, starts at 1
    public float maxDistScaleFactor; //scale factor for closer an object needs to be to be picked up
    public SphereCollider sphere; //base sphere collider
    public LayerMask playerLayer; //layer of the player, used to fix raycasting on pickup
    public LayerMask junkLayer; //layer to set objects to after hitting and losing objects


    float volume; //volume of the katamari

    //Bounds boundingBox; //box that bounds all inner colliders
    public AudioSource sound; //sound source
    public AudioClip plop; //sound made when picking something up
    public AudioClip smack; //sound made when hitting something not meant for pickup
    public float minSpeedForSmack; //minimum speed to make a smack sound
    public float maxAngleForSmack; //minimum angle for speed to cause a smack

    //threshold values
    public int irregularObjectsToGrowth; //number of irregulars to get before growth occurs
    int growthIndex; //current growth index
    public List<float> boundsSizeThreshold; //size the ball needs to be to grow
    //public List<float> growthThreshold; //new radii to set sphere to upon growth
    //public List<float> camDistThreshold; //new distance for the camera on growth
    public List<float> speedThreshold; //new speed on growth
    public List<float> maxSpeedThreshold; //new max speed on growth
    List<Pickupable> irregularObjects; //list of irregular objects

    Vector3 velocity; //velocity of the ball, used to restore speed on pickup
    Rigidbody rb; //rigidbody
    public bool isDashing; //value for dashing, used to set collision for irregulars on pickup during dash

    //Manager
    LevelManager lm;

    // On start, size is set to smallest by default
    private void Start()
    {
        volume = 4f * Mathf.PI / 3f;
        velocity = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        //boundingBox = new Bounds();
        //boundingBox.Encapsulate(sphere.bounds);
        size = 1;
        growthIndex = 0;
        irregularObjects = new List<Pickupable>();
        lm = LevelManager.Instance;
        lm.UpdateSize(sphere.radius);
    }

    /*private void Update()
    {
        DrawBox();
        Debug.Log(boundingBox.size.x * boundingBox.size.y * boundingBox.size.z);
    }*/

    //store velocity on fixed update
    private void FixedUpdate()
    {
        velocity = rb.velocity;
    }

    //on collision enter, if a pickup, pickup
    private void OnCollisionEnter(Collision collision)
    {
        bool pickedUpSuccessfully = false; //picked up?

        Pickupable p = collision.gameObject.GetComponent<Pickupable>();
        //if pickupable isn't null, must be pickup, check if we can get it
        if (p)
        {
            pickedUpSuccessfully = PickUp(p);
        } //else even if not pickupable
        else
        {
            pickedUpSuccessfully = false;
        }

        //if not picked up successfully and in right parameters, play sound
        if (!pickedUpSuccessfully)
        { //previous if is not nested in order to avoid unnecessary calculation
            if (velocity.magnitude >= minSpeedForSmack && Vector3.Angle(velocity, collision.GetContact(0).point - transform.position) <= maxAngleForSmack)
            {
                sound.PlayOneShot(smack);
            }
        }
    }

    //TRIGGER TEST
    private void OnTriggerEnter(Collider collision)
    {
        Pickupable p = collision.gameObject.GetComponent<Pickupable>();
        //if pickupable isn't null, must be pickup, check if we can get it
        if (p)
        {
            PickUp(p);
        }
    }


    //attatches the pickupable object, returns true if successfully picked up
    bool PickUp(Pickupable p)
    {
        //if player size is >= game object size and close enough, add it to the mass
        if (size >= p.size && (sphere.ClosestPoint(p.transform.position) - p.transform.position).magnitude <= sphere.radius * (1 + maxDistScaleFactor))
        {
            //is the object not currently being picked up?
            if (!p.collected)
            {
                //set pickup
                p.PickedUp(transform);

                //play sound
                sound.pitch = Random.Range(.9f, 1.1f);
                sound.PlayOneShot(plop);

                //set bounds encapsulation
                //boundingBox.Encapsulate(p.transform.localPosition);

                //restore velocity
                rb.velocity = velocity;

                //calculate size
                CalculateSize(p);

                //move camera out
                MoveCamera();

                //update lm
                lm.UpdateSize(sphere.radius);

                //disable used collider
                p.defaultCollider.enabled = false;

                //if p is irregular and at the current size increment, add it to the list of changed collisions
                if (p.irregular && p.size == size)
                {
                    //TODO: move pickupable's base transform position to the point of contact
                    p.gameObject.transform.position = sphere.ClosestPoint(p.gameObject.transform.position);

                    irregularObjects.Add(p);
                    if (isDashing) //if dashing, default picked colldier to false
                    {
                        p.pickedCollider.enabled = false;
                    }
                    else //not dashing, enable the new collider
                    {
                        p.pickedCollider.enabled = true;
                    }
                }
                else if (p.irregular)
                {
                    p.pickedCollider.enabled = false;
                }

                return true;
            }
        }

        //if here, not picked up successfully
        return false;
    }

    //moves camera out
    void MoveCamera()
    {
        Camera.main.GetComponentInParent<CameraMover>().distance = (9 + sphere.radius) * CAM_OFFSET_FACTOR;
    }

    //changes max distance and size of the sphere collider
    void CalculateSize(Pickupable p)
    {
        //update volume
        volume += p.volume;
        sphere.radius = Mathf.Pow(volume / (4f * Mathf.PI / 3f), .333f);

        //change size if irregular object count is good AND we meet the current growth requirement
        if (growthIndex < boundsSizeThreshold.Count && 2 * sphere.radius >= boundsSizeThreshold[growthIndex])
        {
            ChangeSize();
        }
    }

    //On size increase, change offset of camera and remove colliders
    void ChangeSize()
    {
        if (growthIndex < boundsSizeThreshold.Count)
        {
            //increase speed and such
            BallMover bm = GetComponentInParent<BallMover>();
            bm.speed = speedThreshold[growthIndex];
            bm.maxSpeed = maxSpeedThreshold[growthIndex];

            //set control values
            growthIndex++;
            size++;

            //tell LM
            lm.PassedSizeThreshold();
        }
        //disable irregular objects' colliders
        foreach(Pickupable p in irregularObjects)
        {
            p.pickedCollider.enabled = false;
        }

        //reset irregular objects list
        irregularObjects = new List<Pickupable>();
    }

    //sets irregular colliders to a specific state
    public void SetIrregulars(bool set)
    {
        foreach(Pickupable p in irregularObjects)
        {
            p.pickedCollider.enabled = set;
        }
    }

    //draws the bounding box
    //Adapted from: https://answers.unity.com/questions/461588/drawing-a-bounding-box-similar-to-box-collider.html
    /*void DrawBox()
    {
        Vector3 v3FrontTopLeft;
        Vector3 v3FrontTopRight;
        Vector3 v3FrontBottomLeft;
        Vector3 v3FrontBottomRight;
        Vector3 v3BackTopLeft;
        Vector3 v3BackTopRight;
        Vector3 v3BackBottomLeft;
        Vector3 v3BackBottomRight;

        Vector3 v3Center = transform.TransformPoint(sphere.center);
        Vector3 v3Extents = boundingBox.extents;

        v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
        v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
        v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
        v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
        v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
        v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
        v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
        v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

        /*v3FrontTopLeft = transform.TransformPoint(v3FrontTopLeft);
        v3FrontTopRight = transform.TransformPoint(v3FrontTopRight);
        v3FrontBottomLeft = transform.TransformPoint(v3FrontBottomLeft);
        v3FrontBottomRight = transform.TransformPoint(v3FrontBottomRight);
        v3BackTopLeft = transform.TransformPoint(v3BackTopLeft);
        v3BackTopRight = transform.TransformPoint(v3BackTopRight);
        v3BackBottomLeft = transform.TransformPoint(v3BackBottomLeft);
        v3BackBottomRight = transform.TransformPoint(v3BackBottomRight);*/
        /*
        Color color = Color.green;

        Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
        Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
        Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

        Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
        Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
        Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
        Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

        Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
        Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
        Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
    }*/
}
