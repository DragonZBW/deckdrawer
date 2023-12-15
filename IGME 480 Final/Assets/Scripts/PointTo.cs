using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PointTo : MonoBehaviour
{
    public Transform target;
    public Vector3 eulerOffset;

    private void Start()
    {
        target = FindObjectOfType<Camera>().transform;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up) * Quaternion.Euler(eulerOffset);
    }
}
