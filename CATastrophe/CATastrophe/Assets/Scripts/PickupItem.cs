using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enables an object to be eligible for being picked up by the cat
[RequireComponent(typeof(Rigidbody))]



public class PickupItem : MonoBehaviour
{
    Rigidbody rb;
    public GameObject player;
    public float launchPower;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    //called when this object is picked up
    public void ChildPickup()
    {
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.AddForce(player.transform.forward * launchPower, ForceMode.Impulse);
    }
    //called when this object is dropped
    public void ChildDrop()
    {
        rb.isKinematic = false;
        transform.SetParent(null);
    }

}
