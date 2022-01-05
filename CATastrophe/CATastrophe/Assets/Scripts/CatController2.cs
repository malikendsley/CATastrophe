using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController2 : MonoBehaviour
{
    //cam integration
    public Transform cam;

    //animation
    public Animator anim;
    public float animWalkTuneSpeed = 10f;


    //locomotion
    public CharacterController controller;
    public float walkSpeed = 5f;
    public float moveDeadzone = 0.1f;
    public float turnSmoothTime = 0.1f;
    private float smoothVelocity;
    private float curspeed;
    private Vector3 thisPos;
    private Vector3 lastPos;
    // Update is called once per frame
    private void Start()
    {
        curspeed = 0;
        lastPos = gameObject.transform.position;
    }
    void Update()
    {
        //get cur pos and calculate speed this frame (useful for animations)
        thisPos = gameObject.transform.position;
        curspeed = Vector3.Distance(new Vector3(thisPos.x, 0, thisPos.z), new Vector3(lastPos.x, 0 , lastPos.z));
        
        //get input and move
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(h, 0, v);

        if(direction.magnitude > moveDeadzone)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
        }
        //manage animator state
        anim.SetFloat("speed", curspeed * animWalkTuneSpeed);

        //housekeeping
        lastPos = thisPos;
    }
}
