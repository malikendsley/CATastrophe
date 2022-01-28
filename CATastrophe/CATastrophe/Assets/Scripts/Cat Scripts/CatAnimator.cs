using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CatAnimator : MonoBehaviour
{

    private Vector3 thisPos;
    private Vector3 lastPos;
    private float curspeed;
    public Animator anim;
    public float animWalkTune;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //currently only handles walking anim
        thisPos = gameObject.transform.position;
        curspeed = Vector3.Distance(new Vector3(thisPos.x, 0, thisPos.z), new Vector3(lastPos.x, 0, lastPos.z));

        anim.SetFloat("speed", curspeed * animWalkTune);
        lastPos = thisPos;


    }
}
