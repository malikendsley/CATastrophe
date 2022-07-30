using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class CatController2 : MonoBehaviour
{

    [SerializeField]
    private Camera cam;
    private Transform camTransform;


    //locomotion
    private CharacterController controller;
    public float walkSpeed = 5f;
    public float moveDeadzone = 0.1f;
    public float turnSmoothTime = 0.1f;
    public bool canMove = true;
    private float smoothVelocity;
    private Vector3 thisPos;
    private Vector3 lastPos;
    public bool canJump = true;

    //taken from Limina
    public LayerMask checkMask;
    public Transform groundCheck;
    public Transform ceilingCheck;
    public float gravity = -9.81f;
    Vector3 velocity;
    public float checkDistance = 0f;
    private bool isCeiling;
    private bool isGrounded;
    public float jumpHeight = 1;

    private bool ceilingLocked = true;
    //pickup system
    [SerializeField]
    private PickupManager pm;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(groundCheck.position, checkDistance);
        Gizmos.color = isCeiling ? Color.green : Color.red;
        Gizmos.DrawSphere(ceilingCheck.position, checkDistance);
    }

    void Start()
    {
        if(cam != null)
        {
            camTransform = cam.transform;
        }
        controller = GetComponent<CharacterController>();
        lastPos = gameObject.transform.position;

    }
    void Update()
    {
        //gather input
        bool interact = Input.GetButtonDown("Interact");
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //get cur pos and calculate speed this frame (useful for animations)
        thisPos = gameObject.transform.position;

        //get input and move
        Vector3 direction = new Vector3(h, 0, v);

        //handle gravity / jumping
        
        isGrounded = Physics.CheckSphere(groundCheck.position, checkDistance, checkMask);
        isCeiling = Physics.CheckSphere(ceilingCheck.position, checkDistance, checkMask);

        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            Debug.Log("Jumped");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        if (isCeiling && !ceilingLocked)
        {
            Debug.Log("Ceiling");
            velocity.y = 0;
            //lock the bump check until we touch the ground again
            ceilingLocked = true;
        }
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        } else
        {
            //if on the ground unlock the bump check
            ceilingLocked = false;
        }
        controller.Move(velocity * Time.deltaTime);

        if (direction.magnitude > moveDeadzone)
        {
            //ternary: don't include cam adjustment if no cam is defined
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + ((cam != null) ? camTransform.eulerAngles.y : 0);
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (canMove)
            {
                controller.Move(Time.deltaTime * walkSpeed * moveDir.normalized);
            }
        }

        //handle interact
        if (interact && pm != null)
        {
            Debug.Log("Try Interact");
            pm.Interact();
        }

        //housekeeping
        lastPos = thisPos;
    }

    public bool GetGrounded()
    {
        return isGrounded;
    }
}
