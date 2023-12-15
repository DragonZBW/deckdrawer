using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBehavior : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10;

    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        targetRotation = Quaternion.Euler(new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180)) * Time.deltaTime) * targetRotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
