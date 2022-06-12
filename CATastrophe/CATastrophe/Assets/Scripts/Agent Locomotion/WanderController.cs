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

    //void OnDrawGizmosSelected()
    //{
    //    ExtraGizmos.DrawWireDisk(transform.position, minRange, Color.green);
    //    ExtraGizmos.DrawWireDisk(transform.position, maxRange, Color.red);
    
    //    var nav = GetComponent<NavMeshAgent>();
    //    if (nav == null || nav.path == null)
    //        return;

    //    var line = this.GetComponent<LineRenderer>();
    //    if (line == null)
    //    {
    //        line = this.gameObject.AddComponent<LineRenderer>();
    //        line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
    //        line.startWidth = line.endWidth = 0.5f;
    //        line.startColor = line.endColor = Color.yellow;
    //    }

    //    var path = nav.path;

    //    line.positionCount = path.corners.Length;

    //    for (var i = 0; i < path.corners.Length; i++)
    //    {
    //        line.SetPosition(i, path.corners[i]);
    //    }

    //}

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
        var randomVector3 = Random.onUnitSphere * Random.Range(minRange, maxRange);
        var randomDirection = new Vector3(randomVector3.x, 0, randomVector3.z);
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out var hit, agent.height * 2, 1);
        return hit.position;
    }           

}
