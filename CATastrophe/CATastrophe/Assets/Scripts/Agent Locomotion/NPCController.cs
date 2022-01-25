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
    public float interactRadius = .5f;
    [Tooltip("Outside of this range the AI will give up on chasing the player")]
    public float detectionRadius = 10;
    [Tooltip("This list is the Pickups that the AI will react to")]
    public GameObject pickup;
    [Tooltip("The AI makes decisions every AITickrate seconds, turn this up for a sluggish but more performant AI")]
    public float AITickrate = .5f;

    [Tooltip("Anchor point for NPC pickups")]
    public GameObject pickupAnchor;

    //only public for debugging
    public bool itemStolen = false;
    public bool carrying = false;
    private int id;
    private NavMeshAgent agent;
    private Vector3 pickupLocation;
    private Vector3 returnLocation;

    private void Start()
    {
        //like and subscribe
        agent = GetComponent<NavMeshAgent>();
        CatPickupManager.PickupEvent += ReactPickup;
        CatPickupManager.DropEvent += ReactDrop;
        id = GetInstanceID();
        returnLocation = new Vector3(transform.position.x, 0, transform.position.z);
        pickupLocation = pickup.transform.position;
        StartCoroutine(AITick());
        agent.speed = NPCSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactRadius);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pickup.transform.position, interactRadius);
    }

    //Choose the highest priority task and perform it for 1 tick
    //TODO Convert this to a state enum
    private IEnumerator AITick()
    {
        while (true)
        {
            yield return new WaitForSeconds(AITickrate);
            //if close enough to pickup location with an item put it down
            if (isPickupInRange() && carrying)
            {
                Debug.Log("Put Down Item");
                NPCDrop(pickup);
                continue;
            }
            else
            //if holding a pickup go back to spawn
            if (carrying)
            {
                Debug.Log("Return Held Item");
                agent.SetDestination(pickupLocation);
                continue;
            }
            else
            //if the player drops an item go pick it up
            if (isCatDetected() && !carrying && !itemStolen && isPickupDisplaced())
            {
                Debug.Log("Retrieve Dropped Item");
                Retrieve();
                continue;
            }
            else
            //if npc in range and in chase mode try to seize item
            if (isInSeizeRange() && itemStolen)
            {
                Debug.Log("Seize Item, In Range");
                StartCoroutine(TrySeize());
                continue;
            }
            else
            //if player holding stolen item chase them
            if (!isInSeizeRange() && isCatDetected() && itemStolen && !carrying)
            {
                Debug.Log("Seek Player");
                agent.SetDestination(cat.transform.position);
                continue;
            }
            else
            if (!isNPCHome())
            //if out of spawn and player out of range go home
            {
                Debug.Log("Return to spawn");
                agent.SetDestination(returnLocation);
                continue;
            }
            else
            //otherwise idle
            {
                Debug.Log("Idle");
            }
        }
    }

    private IEnumerator TrySeize()
    {
        //wait
        yield return new WaitForSeconds(seizeDelay);
        //check if cat is gone
        if (isInSeizeRange())
        {
            Debug.Log("Seize Successful");
            SeizeFromPlayer();
        }
    }

    private void SeizeFromPlayer()
    {
        Debug.Log("Seized From Player");
        carrying = true;
        itemStolen = false;
        cat.GetComponent<PickupManager>().Drop();
        NPCPickup(pickup);
    }

    //move toward pickup or if close enough pick it up
    private void Retrieve()
    {
        if (isPickupDisplaced())
        {
            NPCPickup(pickup);
        }
        else
        {
            agent.SetDestination(new Vector3(pickup.transform.position.x, 0, pickup.transform.position.z));
        }
    }

    private void NPCPickup(GameObject target)
    {
        //state management
        carrying = true;
        itemStolen = false;
        //lock target to anchor
        target.transform.SetParent(pickupAnchor.transform);
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        //call function on pickup
        target.GetComponent<PickupItem>().ChildPickup();


    }

    //can refactor later
    private void NPCDrop(GameObject held)
    {
        //state management
        carrying = false;
        //unlock target
        held.transform.SetParent(null);
        //call function on pickup
        held.GetComponent<PickupItem>().ChildDrop(gameObject);
    }
    #region Checks
    private bool isPickupInRange() => Vector3.Distance(transform.position, pickupLocation) < interactRadius;
    private bool isPickupDisplaced() => Vector3.Distance(pickup.transform.position, pickupLocation) > interactRadius;
    private bool isNPCHome() => Vector3.Distance(transform.position, returnLocation) < homeDist;

    private bool isInSeizeRange() => Vector3.Distance(cat.transform.position, transform.position) < interactRadius;
    private bool isCatDetected() => Vector3.Distance(cat.transform.position, transform.position) < detectionRadius;

    private void ReactPickup(object sender, EventArgs e)
    {
        Debug.Log(sender);
        if (ReferenceEquals(pickup, sender))
        {
            Debug.Log("Pickup matches NPC-" + id + "'s watching list");
            itemStolen = true;
        }
        else
        {
            Debug.Log("NPC-" + id + " ignoring pickup");
        }
    }

    private void ReactDrop(object sender, EventArgs e)
    {
        Debug.Log(sender);
        if (ReferenceEquals(pickup, sender))
        {
            Debug.Log("Drop matches NPC-" + id + "'s watching list");
            itemStolen = false;
        }
        else
        {
            Debug.Log("NPC-" + id + " ignoring drop");
        }
    }
    #endregion
}
