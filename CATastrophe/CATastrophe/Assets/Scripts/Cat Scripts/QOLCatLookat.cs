    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QOLCatLookat : MonoBehaviour
{

    public float targetDist = 5;
    public float gizmoSize = 1f;
    public GameObject headTarget;
    private bool isEnabled = true;
    private Vector3 target;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            Debug.Log("head turned");
            target = Camera.main.transform.forward * targetDist;
            Debug.Log(target);
            headTarget.transform.position = target;
            //headBone.transform.eulerAngles = new Vector3(
            //    Mathf.Clamp(headBone.transform.eulerAngles.x, -1 * Xangle, Xangle),
            //    Mathf.Clamp(headBone.transform.eulerAngles.y, -1 * Yangle, Yangle), 
            //   headBone.transform.eulerAngles.z);
        }
    }

    public void Disable()
    {
        isEnabled = false;
    }
    public void Enable()
    {
        isEnabled = true;
    }
}
