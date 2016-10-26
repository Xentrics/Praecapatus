using System;
using UnityEngine;

public class SideWiseCamera : MonoBehaviour
{
    public Transform target;            // The position that that camera will be following.
    public float turnSpeed = 5f;        // The speed with which the camera will be following.


    public Vector3 offset = Vector3.zero;                     // The initial offset from the target.
    public float pitch = 0.0f;
    public float lookUpOffset = 0.0f;   // how much should we look above the shoulder of the observed player

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
        pitch = newPitch;
        offset.y += (float)Math.Tan(pitch) * Vector2.Distance(new Vector2(target.position.x, target.position.z), new Vector2(transform.position.x, transform.position.z));
    }

    void LateUpdate()
    {
        transform.position = target.position + Assets.Scripts.Constants.gameLogic.worldViewRotation * offset; // adjust world position
        transform.LookAt(target.position + lookUpOffset * Vector3.up, Vector3.up); // adjust rotation
    }
}