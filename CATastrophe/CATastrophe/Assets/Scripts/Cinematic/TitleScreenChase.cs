using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenChase : MonoBehaviour
{

    [SerializeField]
    GameObject NPC;
    [SerializeField]
    GameObject cat;

    
    public Transform leftAnchor;
    public Transform rightAnchor;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //send one cat left, pursued by one person
    //send cat back right, pursuing one person
    //send cat left, pursued by many people
    //send cat right, pursuing many people

}
