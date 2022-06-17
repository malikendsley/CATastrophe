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
    public float Fudge = 1;

    public GameObject CatRef;
    
    public float MinRange = 10;
    public float MaxRange = 20;

    void OnDrawGizmosSelected()
    {
        ExtraGizmos.DrawWireDisk(CatRef.transform.position, MinRange, Color.red);
        ExtraGizmos.DrawWireDisk(CatRef.transform.position, MaxRange, Color.green);
        Gizmos.color = Color.yellow;
        if (agent != null)
        {
            Gizmos.DrawSphere(agent.destination, 1);
        }
    }

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Start()
    {
        //try to set an initial destination
        agent.SetDestination(LooseFollow());
        agent.stoppingDistance = Fudge;
    }

    void Update()
    {
        if (agent.pathPending) return;
        if (!(agent.remainingDistance <= agent.stoppingDistance)) return;
        if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f)
        {
            agent.SetDestination(LooseFollow());
        }
    }   
    
    // Update is called once per frame
    private Vector3 LooseFollow()
    {
        var randomVector2 = RandomPointInAnnulus(CatRef.transform.position, MinRange, MaxRange);
        var randomDirection = new Vector3(randomVector2.x, 0, randomVector2.z); 
        //Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), randomDirection, Quaternion.identity);
        NavMesh.SamplePosition(randomDirection, out var hit, agent.height * 2, 1 | 2);
        return hit.position;
    }

    public Vector3 RandomPointInAnnulus(Vector3 origin, float minRadius, float maxRadius)
    {
        var randomDistance = Random.Range(minRadius, maxRadius);

        var point = origin + (Random.insideUnitSphere * randomDistance);

        return point;
    }

}
