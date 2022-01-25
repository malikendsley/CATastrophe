using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enables an object to be eligible for being picked up by the cat
//notifies other objects when it is picked up (TODO: Implement a subscriber system)

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]


public class PickupItem : MonoBehaviour
{
    Rigidbody rb;
    public float launchPower;
    public Vector3 ownOffsetPos;
    public Vector3 ownOffsetRot;
    private bool beingHeld = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnDrawGizmosSelected()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Gizmos.color = Color.red;
        if (!beingHeld  && mf != null)
        {
            Gizmos.DrawWireMesh(mf.sharedMesh, transform.position + ownOffsetPos, transform.localRotation * Quaternion.Euler(ownOffsetRot));
        }
    }
    //called AFTER this object is picked up and locked  
    public void ChildPickup()
    {
        beingHeld = true;
        rb.isKinematic = true;
        gameObject.transform.localPosition = ownOffsetPos;
        gameObject.transform.localRotation = Quaternion.Euler(ownOffsetRot);

    }
    //called AFTER this object is unlocked and dropped
    public void ChildDrop(GameObject whoDropped)
    {
        beingHeld = false;
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.AddForce(whoDropped.transform.forward * launchPower, ForceMode.Impulse);

    }

}
