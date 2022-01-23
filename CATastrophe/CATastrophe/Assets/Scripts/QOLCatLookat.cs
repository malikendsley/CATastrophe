    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QOLCatLookat : MonoBehaviour
{
    public float Xangle = 30;
    public float Yangle = 50;
    public float targetDist = 5;
    public float gizmoSize = 1f;
    public GameObject headBone;
    public GameObject cam;
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
            target = cam.transform.forward;
            Debug.Log(target);
            headBone.transform.LookAt(target * targetDist);
            //headBone.transform.eulerAngles = new Vector3(
            //    Mathf.Clamp(headBone.transform.eulerAngles.x, -1 * Xangle, Xangle),
            //    Mathf.Clamp(headBone.transform.eulerAngles.y, -1 * Yangle, Yangle), 
            //   headBone.transform.eulerAngles.z);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target * targetDist, gizmoSize);
        Gizmos.DrawLine(cam.transform.position, target * targetDist);
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
