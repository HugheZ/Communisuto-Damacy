using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public int size; //size of the pickup, [1,5] = [small, gargantuan]
    public bool irregular; //if irregular, changes collision
    public float volume; //how big the object is
    public bool collected; //has the object been collected

    //collider values
    //public Collider usedCollider; //collider currently in use, default if not picked up, picked if so
    public Collider defaultCollider; //collider to be used when on the ground, not picked up
    public Collider pickedCollider; //collider to be used when picked up

    //On start, set up colliders
    private void Start()
    {
        //usedCollider = defaultCollider;
        defaultCollider.enabled = true;
        if(irregular) pickedCollider.enabled = false;
    }

    //controls gameobject-centric pickup logic
    public void PickedUp(Transform picker)
    {
        MouseController mouse = GetComponent<MouseController>();
        if (mouse) mouse.PickedUp();

        //set standard vals
        collected = true;
        gameObject.transform.SetParent(picker);
        gameObject.layer = LayerMask.NameToLayer("Player");

        /*//do collider change if irregular
        if (irregular)
        {
            usedCollider = pickedCollider;
            defaultCollider.enabled = false;
            pickedCollider.enabled = true;
        }
        else //else just disable collider
        {
            defaultCollider.enabled = false;
        }*/
    }

    //controls gameobject-centric drop logic
    public void Dropped()
    {
        MouseController mouse = GetComponent<MouseController>();
        if (mouse) mouse.enabled = true;

        //set standard vals
        collected = false;
        gameObject.transform.parent = null;
        gameObject.layer = LayerMask.NameToLayer("Default");

        /*//do collider change
        if (irregular)
        {
            usedCollider = defaultCollider;
            defaultCollider.enabled = true;
            pickedCollider.enabled = false;
        }
        else //else just enable default collider
        {
            defaultCollider.enabled = true;
        }*/
    }
}
