using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    public OVRInput.Controller Controller;
    public string buttonName;
    private bool grabbing; //whether player is grabbing an object
    public float grabRadius; //the range of the sphere cast
    public LayerMask grabMask; //a layer mask such that only objects in that layer can be grabbed.

    private GameObject grabbedObject; //object being held by player

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!grabbing && Input.GetAxis(buttonName) == 1)
        {
            GrabObject();
        }
        if(grabbing && Input.GetAxis(buttonName) < 1)
        {
            DropObject();
        }
    }

    void GrabObject()
    {
        grabbing = true; //player/controller grabbed an object

        RaycastHit[] hits; //array to store all raycastHits - detect object(s)

        //only react for objects in the correct layer(s)
        hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);
        //if there is at least one object (projectile) in the area:
        if (hits.Length > 0)
        {
            int closestHit = 0; //randomly defined integer
            //Loop through all objects (being detected - by Raycast) to find closest object to player/controller:
            for (int i = 0; i<hits.Length; i++)
            {
                //if current object is closer to the player/controller, as compared to the previous object:
                if ( hits[i].distance < hits[closestHit].distance )
                {
                    closestHit = i;
                }
            }

            grabbedObject = hits[closestHit].transform.gameObject; //get the gameObject related to the RaycastHit
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true; //make the rigidbody of the currently grabbedObject a KINEMATIC rigidbody - so that gravity doesn't work on it while it is being held (ie. it will not be pulled to the ground)
            grabbedObject.transform.position = transform.position; //change the grabbedObject's position to the position of the controller (ie. let the player grab the object)
            grabbedObject.transform.parent = transform; //make the controller to be the new parent of the gameObject (it is grabbing) - so that they  will move together
        }

    }

    //reverse everything in GrabObject() function:
    void DropObject()
    {
        grabbing = false; //player now is not grabbing any object (after dropping it)

        if(grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false; //make object fall to ground
            //set velocity of falling object:
            grabbedObject.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(Controller);
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);
            //dereference the object(that fell):
            grabbedObject = null; //once the player/controller drops the object, then dereference the object
        }

        //currently, there are no objects being grabbed
    }
}
