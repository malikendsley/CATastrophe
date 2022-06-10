using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CatAnimator : MonoBehaviour
{

    [SerializeField]
    CatController2 cc;

    private Vector3 thisPos;
    private Vector3 lastPos;
    private float curspeed;
    public Animator anim;
    public float animWalkTune = 500;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //find out how far you've walked since last frame
        thisPos = gameObject.transform.position;
        curspeed = Vector3.Distance(new Vector3(thisPos.x, 0, thisPos.z), new Vector3(lastPos.x, 0, lastPos.z));

        //find out where in the jump arc you are
        float dy = thisPos.y - lastPos.y;
        float vertSpeed = Mathf.Clamp(dy * animWalkTune, -1, 1);
        anim.SetFloat("vertical speed", vertSpeed);
        anim.SetFloat("speed", curspeed * animWalkTune);
        bool grounded = cc.GetGrounded();
        anim.SetBool("grounded", grounded);

        Debug.Log("Speed: " + curspeed * animWalkTune + " Vert: " + vertSpeed + " Grounded: " + grounded);

        lastPos = thisPos;

    }
}
