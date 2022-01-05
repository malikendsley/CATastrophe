using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHeld : MonoBehaviour
{
    public Transform player, ballContainer;
    public float pickUpRange;
    public static bool full;
    public Rigidbody rb;
    public BoxCollider coll;
    public float dropForwardForce;

    private void Start()
    {
        //Setup
        if(!full)
        {
            rb.isKinematic = false;
            //coll.isTrigger = false;
        }
        else
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            //coll.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if(!full && distanceToPlayer.magnitude <= pickUpRange && Input.GetKey(KeyCode.Space) && !full)
        {
            PickUp();
        }
        if(full && Input.GetKeyDown(KeyCode.Space))
        {
            Drop();
        }
    }

    private void PickUp()
    {
        full = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        //coll.isTrigger = true;

        //Make ball a child of the cat and move it to default position
        transform.SetParent(ballContainer);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    private void Drop()
    {
        full = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        //coll.isTrigger = false;

        //Ball carries player momentum
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        //AddForce
        rb.AddForce(player.forward * dropForwardForce, ForceMode.Impulse);

        //Random rotation whilst throwing
        //float random = random.Range(-1f, 1f);
        //rb.AddTorque(new Vector3(random, random, random) * 10);
    }
}