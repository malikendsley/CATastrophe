using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClampedLookAt : MonoBehaviour
{
    public GameObject trackedObject;
    private GameObject originalObject;

    public Transform target1;
    public Transform target2;

    public bool smooth = true;
    public float damping = 3;
    private float originalDamping;


    // Start is called before the first frame update
    private void Start()
    {
        originalObject = trackedObject;
        originalDamping = damping;
    }
    private void OnDrawGizmos()
    {
        //draw the bounds if display bounds is selected
        if (target1 != null && target2 != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target1.position, target2.position);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (target1 != null && target2 != null)
        {
            Debug.DrawLine(trackedObject.transform.position, ExtensionMethods.GetClosestPointOnFiniteLine(trackedObject.transform.position, target1.position, target2.position));
        }
    }

    // Update is called once per frame
    void Update()
    {

        //get closest point on line to target
        //slerp towards that point
        if (smooth)
        {
            var closestPoint = ExtensionMethods.GetClosestPointOnFiniteLine(trackedObject.transform.position, target1.position, target2.position);
            var rotation = Quaternion.LookRotation(closestPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, damping * Time.deltaTime);
        }
    }

    public void UpdateTransform(GameObject transform, float damp)
    {
        damping = damp;
        trackedObject = transform;
    }

    public void ResetTransform()
    {
        damping = originalDamping;
        trackedObject = originalObject;
    }

    public void SetSmooth(bool smooth)
    {
        this.smooth = smooth;
    }
}
