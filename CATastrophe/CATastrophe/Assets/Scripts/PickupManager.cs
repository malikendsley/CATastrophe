using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public class PickupManager : MonoBehaviour
{
    public Vector3 pickupAnchor;

    public Vector3 colliderCenter;
    public float colliderRadius;    
    private bool isHolding;
    private SphereCollider rb;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the object is a pickup, add it to a list of eligible pickups
    }

    private void OnTriggerExit(Collider other)
    {
        //if the object is a pickup, remove it from the list of eligible pickups
    }

    private void PickUp()
    {
        //prioritize the object the player is looking at
        //cast a ray normal to the center of the screen
        //retrieve the closest eligible item and pick it up
            //either destroy and instantiate the pickup or Lerp it and child it
    }
    
    private void Drop()
    {
        //release the held item and toss it a short distance away (csgo style)
        //disable isKinematic on the target rigidbody and apply an away force
            //if you use a force system, then make the force applied variable
    }

    //TODO: Deposit? Submit? Use?
}
