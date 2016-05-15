using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CameraPlayer : MonoBehaviour
{
    public Transform target;            // The position that that camera will be following.
    public float turnSpeed = 5f;        // The speed with which the camera will be following.


    public Vector3 offset = Vector3.zero;                     // The initial offset from the target.
    public float pitch = 0.0f;


    void Start()
    {
        // Calculate the initial offset.
        print("Camera start.");
        if (target == null) throw new NullReferenceException("Camera has not target!");
        if (offset == Vector3.zero)
            offset = transform.position - target.position;

        setPitch(pitch);
    }

    /*
     * set up the pitch of the camera relative to the player indipendent of other rotations
     * this works by setting the correct height (y-axis), so that "lookAt" sets the correct angle during "lateUpdate"
     */
    public void setPitch(float newPitch)
    {
        this.pitch = newPitch;
        offset.y = offset.y + (float)Math.Tan(pitch) * Vector2.Distance(new Vector2(target.position.x, target.position.z), new Vector2(transform.position.x, transform.position.z));
    }

    void LateUpdate()
    {
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        transform.position = target.position + offset;
        transform.LookAt(target.position, Vector3.up);
    }
}