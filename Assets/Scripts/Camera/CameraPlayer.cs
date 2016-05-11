using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CameraPlayer : MonoBehaviour
{
    public Transform target;            // The position that that camera will be following.
    public float turnSpeed = 5f;        // The speed with which the camera will be following.


    Vector3 offset;                     // The initial offset from the target.


    void Start()
    {
        // Calculate the initial offset.
        print("Camera start.");
        if (target == null) throw new NullReferenceException("Camera has not target!");
        offset = transform.position - target.position;
    }


    void LateUpdate()
    {
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }
}