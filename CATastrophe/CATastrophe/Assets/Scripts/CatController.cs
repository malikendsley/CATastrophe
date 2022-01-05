using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{

    public GameObject catmesh;
    private CharacterController cat;
    public float speed;
    Vector3 move;
    private Animator anim;
    bool walking = false;

    public float maxHappy = 100;
    public float currentHappy;
    public HappyBar happyBar;

    // Start is called before the first frame update
    void Start()
    {
        cat = GetComponent<CharacterController>();
        move = Vector3.zero;
        anim = catmesh.GetComponent<Animator>();

        currentHappy = maxHappy;
        happyBar.SetMaxHappy(maxHappy);
    }

    // Update is called once per frame
    void Update()
    {

        //Testing the happiness bar slider
        if(Input.GetKey(KeyCode.B))
        {
            changeHappy(20 * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.N))
        {
            changeHappy(-20);
        }


        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            move += Vector3.forward;
            walking = true;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            move += Vector3.back;
            walking = true;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            move += Vector3.left;
            walking = true;

        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            move += Vector3.right;
            walking = true;

        }
        if(move == Vector3.zero)
        {
            walking = false;
        }
        if (walking)
        {
            anim.SetFloat("speed", 1);
        } else
        {
            anim.SetFloat("speed", 0);
        }
        cat.Move(move.normalized * speed * Time.deltaTime);
        move = Vector3.zero;

    }

    void changeHappy(float happiness)
    {
        currentHappy += happiness;
        happyBar.SetHappy(currentHappy);
    }
}
