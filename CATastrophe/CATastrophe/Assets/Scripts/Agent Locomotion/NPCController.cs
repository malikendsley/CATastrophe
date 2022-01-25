using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [Tooltip("Until a Service model is implemented for the pickup system this reference is necessary")]
    public PickupManager CatPickupManager;
    public GameObject cat;

    public float seizeDelay = 1;
    public float NPCSpeed = 1;

    [Tooltip("The range at which the NPC is considered \"home.\" this makes for less stutter")]
    public float homeDist = .25f;
    [Tooltip("The range at which the NPC will try to seize their pickup")]
    public float seizeRadius = .5f;
    [Tooltip("Outside of this range the AI will give up on chasing the player")]
    public float detectionRadius = 10;
    [Tooltip("This list is the Pickups that the AI will react to")]
    public GameObject pickup;
    [Tooltip("The AI will go back over here if it has nothing else to do")]
    public Vector3 returnLocation;
    [Tooltip("The AI makes decisions every AITickrate seconds, turn this up for a sluggish but more performant AI")]
    public float AITickrate = .5f;

    [Tooltip("Anchor point for NPC pickups")]
    public GameObject pickupAnchor;

    //only public for debugging
    public bool active = false;
    public bool itemStolen = false;
    public bool carrying = false;
    public bool returning = false;
    private int id;
    private NavMeshAgent agent;


    void Start()
    {
        //like and subscribe
        agent = GetComponent<NavMeshAgent>();
        CatPickupManager.PickupEvent += ReactPickup;
        CatPickupManager.DropEvent += ReactDrop;
        id = GetInstanceID();
        returnLocation = new Vector3(transform.position.x, 0, transform.position.z);
        StartCoroutine(AITick());
        agent.speed = NPCSpeed;
    }

    //Choose the highest priority task and perform it for 1 tick
    //TODO Convert this to a state enum
    IEnumerator AITick()
    {
        while (true)
        {
            yield return new WaitForSeconds(AITickrate);
            //if close enough to spawn with an item put it down
            if (HomeCheck() && carrying)
            {
                Debug.Log("Priority 1. Put Down Item");
                NPCDrop(pickup);
                continue;
            }
            else
            //if holding a pickup go back to spawn
            if (carrying)
            {
                Debug.Log("Priority 2. Return Held Item");
                agent.SetDestination(returnLocation);
                continue;
            }
            else
            //if the player drops an item go pick it up
            if (DetectionCheck() && !carrying && !itemStolen && returning)
            {
                Debug.Log("Priority 3. Retrieve Dropped Item");
                Retrieve();
                continue;
            }
            else
            //if npc in range and in chase mode try to seize item
            if (SeizeCheck() && itemStolen)
            {
                Debug.Log("Priority 4. Seize Item, In Range");
                StartCoroutine(TrySeize());
                continue;
            }
            else
            //if player holding stolen item chase them
            if (!SeizeCheck() && DetectionCheck() && itemStolen && !carrying)
            {
                Debug.Log("Seek Player");
                agent.SetDestination(cat.transform.position);
                continue;
            }
            else
            if (!HomeCheck())
            //if out of spawn and player out of range go home
            {
                Debug.Log("Return to spawn");
                agent.SetDestination(returnLocation);
                continue;
            }
            else
            //otherwise idle
            {
                Debug.Log("Priority 7. Idle");
            }
        }
    }

    IEnumerator TrySeize()
    {
        //wait
        yield return new WaitForSeconds(seizeDelay);
        //check if cat is gone
        if (SeizeCheck())
        {
            Debug.Log("Seize Successful");
            SeizeFromPlayer();
        }
    }
    void SeizeFromPlayer()
    {
        Debug.Log("Seized From Player");
        carrying = true;
        itemStolen = false;
        cat.GetComponent<PickupManager>().Drop();
        NPCPickup(pickup);
    }

    //move toward pickup or if close enough pick it up
    void Retrieve()
    {
        if (Vector3.Distance(transform.position, pickup.transform.position) < seizeRadius)
        {
            NPCPickup(pickup);
        }
        else
        {
            agent.SetDestination(new Vector3(pickup.transform.position.x, 0, pickup.transform.position.z));
            returning = true;
        }
    }

    void NPCPickup(GameObject target)
    {
        //state management
        carrying = true;
        //lock target to anchor
        target.transform.SetParent(pickupAnchor.transform);
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        //call function on pickup
        target.GetComponent<PickupItem>().ChildPickup();


    }

    //can refactor later
    void NPCDrop(GameObject held)
    {
        //state management
        carrying = false;
        //unlock target
        held.transform.SetParent(null);
        //call function on pickup
        held.GetComponent<PickupItem>().ChildDrop(gameObject);
    }
    bool HomeCheck()
    {
        return (Vector3.Distance(transform.position, returnLocation) < homeDist);
    }
    bool SeizeCheck()
    {
        if (Vector3.Distance(cat.transform.position, transform.position) < seizeRadius)
        {
            //Debug.Log("Seize In Range");
            return true;
        }
        else
        {
            //Debug.Log("Seize Not In Range");
            return false;
        }
    }
    bool DetectionCheck()
    {
        //Debug.Log("Out of range");
        return Vector3.Distance(cat.transform.position, transform.position) < detectionRadius;
    }
    void ReactPickup(object sender, EventArgs e)
    {
        Debug.Log(sender);
        if (ReferenceEquals(pickup, sender))
        {
            Debug.Log("Pickup matches NPC-" + id + "'s watching list");
            itemStolen = true;
            active = true;
        }
        else
        {
            Debug.Log("NPC-" + id + " ignoring pickup");
        }
    }
    void ReactDrop(object sender, EventArgs e)
    {
        Debug.Log(sender);
        if (ReferenceEquals(pickup, sender))
        {
            Debug.Log("Drop matches NPC-" + id + "'s watching list");
            itemStolen = false;
            active = false;
        }
        else
        {
            Debug.Log("NPC-" + id + " ignoring drop");
        }
    }
}
