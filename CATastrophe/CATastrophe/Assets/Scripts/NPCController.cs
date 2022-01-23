using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NPCController : MonoBehaviour
{
    [Tooltip("Turning this up makes the AI closer to frame perfect")]
    public float difficultyTuner;
    [Tooltip("Outside of this range the AI will give up on chasing the player")]
    public float detectionRadius;
    [Tooltip("This list is the Pickups that the AI will react to")]
    public List<GameObject> pickups;
    [Tooltip("The AI will go back over here if it has nothing else to do")]
    public Vector3 originalLocation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
