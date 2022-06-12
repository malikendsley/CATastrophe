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
    
    public float searchRange = 10;
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
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            agent.SetDestination(ChooseRandomLocation());
        }
    }
    
    // Update is called once per frame
    Vector3 ChooseRandomLocation()
    {
        var randomDirection = Random.insideUnitSphere * searchRange;
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out var hit, searchRange, 1);
        return hit.position;
    }

}
