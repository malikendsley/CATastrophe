using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WanderController : MonoBehaviour
{
    private NavMeshAgent agent;

    [Tooltip("Distance() is slow, and we have a lot of agents. " +
             "This is a fudge factor for checking agent arrival. " +
             "Turn this up if you find agents get stuck.")]
    public float fudge = 1;
    
    public float minRange = 10;
    public float maxRange = 20;

    void OnDrawGizmosSelected()
    {
        ExtraGizmos.DrawWireDisk(transform.position, minRange, Color.red);
        ExtraGizmos.DrawWireDisk(transform.position, maxRange, Color.green);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(agent.destination, 1);
    }

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Start()
    {
        //try to set an initial destination
        agent.SetDestination(ChooseRandomLocation());
    }

    void Update()
    {
        if (agent.pathPending) return;
        if (!(agent.remainingDistance <= agent.stoppingDistance)) return;
        if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f)
        {
            agent.SetDestination(ChooseRandomLocation());
        }
    }
    
    // Update is called once per frame
    Vector3 ChooseRandomLocation()
    {
        var randomVector3 = Random.onUnitSphere * Random.Range(minRange, maxRange);
        var randomDirection = new Vector3(randomVector3.x, 0, randomVector3.z);
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out var hit, agent.height * 2, 1);
        return hit.position;
    }           

}
