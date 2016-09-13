﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Assets.Scripts.Entity
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider))]
    public class EntityMovement : MonoBehaviour
    {
        public float moveSpeed = 6f;            // The speed that the player will move at
        public float runSpeed = 24f;    // The speed that the player will move at.
        public float turnSpeed = 0.25f;         // The rate at which the player can turn around (independent of the camera)
        public float jumpStrength = 12f;        // This values defines with respect to the characters weight how high he can jump
        public float gravityMultiplier = 4f;    // Additional gravity that can be applied

        protected Vector3 velocity;                        // The vector to store the direction of the player's movement.
        protected Vector3 inputVelocity;                   // The amount of movement caused by mouse/keyboard input
        protected Vector3 lookDir;                         // 

        public float GroundCheckDistance = 0.1f;
        protected float OrigGroundCheckDistance;
        public Vector3 groundCheckOffset;

        protected Animator animComp;
        protected Rigidbody rigitBodyComp;                 // Reference to the player's rigidbody.
        protected SpriteRenderer spriteRenderer;           // lets us flit stuff easily
        protected Collider mainCollider;
        protected int floorMask;                           // A layer mask so that a ray can be cast just at gameobjects on the floor layer.

        // character states
        public bool isInAir = false;                       // shall be true, if the player is in the air (for whatever reason)
        protected bool canJump = true;                     // can be false duo to exhaustion and other reasons
        protected bool bRun = true;
        protected bool canRun = true;                      // can be false duo to exhaustion and other reasons

        virtual protected void Awake()
        {
            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask("Floor");
            lookDir = new Vector3();
            velocity = new Vector3();
            inputVelocity = new Vector3();
            OrigGroundCheckDistance = GroundCheckDistance;

            // Set up references. They cannot be null since they are required!
            rigitBodyComp = GetComponent<Rigidbody>();
            animComp = GetComponent<Animator>();
            mainCollider = GetComponent<Collider>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        void FixedUpdate()
        {
            // check if we are in the air
            CheckGroundStatus();

            if (!isInAir)
            {
                // we stand on the ground
                //inputVelocity.x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
                //inputVelocity.z = CrossPlatformInputManager.GetAxisRaw("Vertical");

                if (canJump)
                {
                    // AI jump
                    throw new NotImplementedException("Make AI Jump!");
                    /*
                     * de-comment on implement
                    Vector3 viewVelocity = Quaternion.LookRotation(lookDir * turnSpeed) * inputVelocity;
                    rigitBodyComp.velocity = new Vector3(rigitBodyComp.velocity.x + viewVelocity.x * moveSpeed, jumpStrength, rigitBodyComp.velocity.z + viewVelocity.z * moveSpeed);
                    isInAir = true;
                    animatorComp.applyRootMotion = false;
                    */
                }
            }

            // Move the player around the scene.
            if (isInAir)
                Fall();
            else
                Move(inputVelocity.x, inputVelocity.z, bRun);

            // Turn the player to face the mouse cursor.
            Turning();

            // Animate the player.
            Animating(inputVelocity.x, inputVelocity.z);
        }

        protected void Fall()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rigitBodyComp.AddForce(extraGravityForce);
            //GroundCheckDistance = rigitBodyComp.velocity.y < 0 ? OrigGroundCheckDistance : 0.01f;
        }


        protected void Move(float h, float v, bool run)
        {
            // Set the movement vector based on the axis input.
            velocity.Set(h, 0f, v);

            // Normalise the movement vector and make it proportional to the speed per second.
            velocity = velocity.normalized * ((bRun) ? runSpeed : moveSpeed) * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            rigitBodyComp.MovePosition(transform.position + Quaternion.LookRotation(lookDir * turnSpeed) * velocity);
        }


        protected void Turning()
        {
            transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.LookRotation(lookDir), turnSpeed);
        }


        protected void Animating(float h, float v)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f || v != 0f;

            if (h != 0)
            {
                spriteRenderer.flipX = (h > 0) ? false : true;
                animComp.SetInteger("direction", (h > 0) ? (int)EDirection.EAST : (int)EDirection.WEST); // keep the current look direction if h == 0!
            }

            if (walking)
            {
                if (bRun)
                {
                    animComp.SetBool("running", true);
                } 
                else
                {
                    animComp.SetBool("running", true);
                }
            }
            else
            {
                animComp.SetBool("running", false);
            }
        }

        /**
         * check if the player is still in the air using ray casts
         */
        protected void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + groundCheckOffset + (Vector3.up * 0.1f), transform.position + groundCheckOffset + (Vector3.up * 0.1f) + (Vector3.down * GroundCheckDistance), Color.red, 5f);
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // we ignore the entity plane. Come up with a better idea later...
            int oldlayer = this.gameObject.layer;
            //this.gameObject.layer = 2; // temporarily set to "ignore raycast" layer
            if (Physics.Raycast(transform.position + groundCheckOffset + (Vector3.up * 0.1f), Vector3.down, out hitInfo, GroundCheckDistance))
            {
                // hit something.
                isInAir = false;
                animComp.SetBool("grounded", true);
            }
            else
            {
                isInAir = true;
                animComp.SetBool("grounded", false);
            }
            //this.gameObject.layer = oldlayer;
        }

        public void toggleRunning()
        {
            bRun = !bRun;
        }

        public void setIsRunning(bool b)
        {
            bRun = b;
        }

        public virtual Vector3 getPosition()
        {
            return rigitBodyComp.position;
        }
    }
}
