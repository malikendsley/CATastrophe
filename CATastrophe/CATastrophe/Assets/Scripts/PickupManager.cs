using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* works in tanden with PickupItem to allow certain objects to be picked up
 * the manager takes care of the structure, the children do the movement
 * needs housekeeping but im heading out for now
*/

[RequireComponent(typeof(SphereCollider))]

public class PickupManager : MonoBehaviour
{
    //using this as a gameobject will manage transform automagically
    public GameObject pickupAnchor;
    public Camera cam;
    public Vector3 colliderCenter;
    public float colliderRadius;
    public float pickupDistance;
    private bool isHolding;
    private List<GameObject> pickups;
    private GameObject held;
    RaycastHit rh;

    int pickupMask = LayerMask.GetMask("Pickup");

    private void OnTriggerEnter(Collider pickup)
    {
        //if the object is a pickup, add it to a list of eligible pickups
        if (pickup.gameObject.tag == "Pickup")
        {
            pickups.Add(pickup.gameObject);
            Debug.Log(pickup.gameObject.name + " In Range");
        }
    }

    private void OnTriggerExit(Collider pickup)
    {
        //if the object is a pickup, remove it from the list of eligible pickups
        if (pickup.gameObject.tag == "Pickup")
        {
            pickups.Remove(pickup.gameObject);
            Debug.Log(pickup.gameObject.name + " Out of Range");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(pickupAnchor.transform.position, "anchor.png", true);
        Gizmos.DrawWireSphere(colliderCenter, colliderRadius);
    }


    public void OnInteract()
    {
        if (!isHolding)
        {
            //when nothing is held, the script behaves straightforward
            //priotize looked at object
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rh, pickupDistance, pickupMask))
            {
                Debug.DrawRay(cam.transform.position, cam.transform.forward * pickupDistance, Color.yellow);
                rh.transform.SetParent(pickupAnchor.transform);
                rh.transform.localPosition = Vector3.zero;
                rh.transform.gameObject.GetComponent<PickupItem>().ChildPickup();
                held = rh.transform.gameObject;
                isHolding = true;
            }
            
            //next, try to pick up the closest eligible gameobject
            else 
            {
                //this might be cleanable with Linq system, 
                if(pickups.Count == 0) {
                    Debug.Log("Nothing in Range");
                    return;
                }
                //set to null to make the compiler happy but this will always be set, so no big deal
                GameObject closest = null;
                float dist = Mathf.Infinity;
                foreach(GameObject pickup in pickups)
                {
                    if (Vector3.Distance(pickup.transform.position, gameObject.transform.position) < dist)
                    {
                        closest = pickup;
                    }
                }
                closest.transform.SetParent(pickupAnchor.transform);
                closest.transform.localPosition = Vector3.zero;
                closest.transform.gameObject.GetComponent<PickupItem>().ChildPickup();
                held = closest.transform.gameObject;
                isHolding = true;
            }
        }
        else
        {
            //when an object is held, proximity pickups don't function but directed ones do
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rh, pickupDistance, pickupMask))
            {
                Debug.DrawRay(cam.transform.position, cam.transform.forward * pickupDistance, Color.yellow);
                rh.transform.SetParent(pickupAnchor.transform);
                rh.transform.localPosition = Vector3.zero;
                rh.transform.gameObject.GetComponent<PickupItem>().ChildPickup();
                held = rh.transform.gameObject;
                isHolding = true;
            } else
            {
                Drop();
            }

            //down here, something is held and nothing is being looked at so drop the currently held object
            
        }


    }

    private void Drop()
    {

        held.GetComponent<PickupItem>().ChildDrop();
        isHolding = false;
        held = null;
    }

    //TODO: Deposit? Submit? Use?

    public GameObject GetHeld()
    {
        return held;
    }
}
