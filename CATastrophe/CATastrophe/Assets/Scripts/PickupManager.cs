using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* works in tanden with PickupItem to allow certain objects to be picked up
* the manager takes care of the positions, the children do the kinematics
*/

public class PickupManager : MonoBehaviour
{
    //using this as a gameobject will manage transform automagically
    public GameObject pickupAnchor;
    public Camera cam;
    public Vector3 colliderCenter;
    public float pickupDistance;
    private bool isHolding;
    private List<GameObject> pickups = new List<GameObject>();
    private GameObject held = null;
    private SphereCollider sc;
    RaycastHit rh;
    int pickupMask;
    private void Start()
    {
        pickupMask = LayerMask.GetMask("Pickup");
        sc = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        sc.radius = pickupDistance;
        sc.center = colliderCenter;
        sc.isTrigger = true;

    }

    void OnTriggerEnter(Collider pickup)
    {
        //if the object is a pickup, add it to a list of eligible pickups
        if (pickup.gameObject.tag == "Pickup")
        {
            pickups.Add(pickup.gameObject);
            Debug.Log(pickup.gameObject.name + " In Range");
        } else
        {
        Debug.Log(pickup.gameObject.name + "Not a pickup");
        }
    }

    void OnTriggerExit(Collider pickup)
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
        Gizmos.DrawWireSphere(gameObject.transform.position + colliderCenter, pickupDistance);
    }


    public void Interact()  
    {
        if (isHolding)
        {
            if (!camPickup())
            {
                Drop();
            }
        } else
        {
            if (!camPickup())
            {
                proxPickup();
            }
        }
        Debug.Log("Nothing nearby");
    }
    private void Drop()
    {

        isHolding = false;
        held.transform.SetParent(null);
        held.GetComponent<PickupItem>().ChildDrop(gameObject);
        held = null;
        
    }

    //return true on successful pickup
    private bool proxPickup()
    {
        if(pickups.Count == 0)
        {
            return false;
        } else
        {
            //get closest eligible
            GameObject closest = null;
            float minDist = Mathf.Infinity;
            foreach(GameObject pickup in pickups)
            {
                if (Vector3.Distance(pickup.transform.position, gameObject.transform.position) < minDist)
                {
                    closest = pickup;
                }
            }
            //pick it up
            Pickup(closest);
            return true;
        }
    }
    //return true on successful pickup
    private bool camPickup()
    {
        //while the ray may hit something it also needs to be within the pickup radius to be consistent
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rh, pickupMask) && pickups.Contains(rh.collider.gameObject))
        {
            Pickup(rh.transform.gameObject);
            return true;
        } else
        {
            Debug.Log("Cam failed, none in range");
            return false;
        }
    }

    private void Pickup(GameObject target)
    {
        //lock target to anchor
        target.transform.SetParent(pickupAnchor.transform);
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        held = target;
        isHolding = true;
        //notify the pickup
        target.gameObject.GetComponent<PickupItem>().ChildPickup(gameObject);
    }
    public GameObject GetHeld()
    {
        return held;
    }
}
