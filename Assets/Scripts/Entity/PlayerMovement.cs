using Assets.Scripts.Entity;
using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Assets.Scripts.Entity
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : EntityMovement
    {
        public Camera followCamera;              // The default third person camera

        public bool freeLook = false;           // If true, the rotation of the player will adjust to the cameras rotation. May be disabled by pressing/holding a certain key
        bool cinimaticModeOn = false;     // disable free movement etc. for cinematic actions


        protected override void Awake()
        {
            base.Awake();
        }


        public void setCinematicMode(bool turnOn)
        {
            cinimaticModeOn = turnOn;
            if (turnOn)
            {
                inputVelocity.Set(0, 0, 0);
                // we may not disable basic abilities like "canJump" here, as they may interfere with actual cinematics!
            }
            else
            {
            }
        }

        // complete override
        void FixedUpdate()
        {
            // check if we are in the air
            CheckGroundStatus();

            // correct player mesh rotation based on the camera rotation
            if ((followCamera != null) && (!freeLook))
            {
                lookDir = transform.position - followCamera.transform.position;
                lookDir.y = 0; // we don't need y. yet.
            }

            if (!cinimaticModeOn)
            {
                if (!isInAir)
                {
                    // we stand on the ground
                    inputVelocity.x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
                    inputVelocity.z = CrossPlatformInputManager.GetAxisRaw("Vertical");


                    if (canJump && CrossPlatformInputManager.GetButton("Jump"))
                    {
                        // we jump - apply some upward velocity + the current planar movement direction
                        print("jumped");
                        Vector3 viewVelocity = Quaternion.LookRotation(lookDir * turnSpeed) * inputVelocity;
                        rigitBodyComp.velocity = new Vector3(rigitBodyComp.velocity.x + viewVelocity.x * moveSpeed, jumpStrength, rigitBodyComp.velocity.z + viewVelocity.z * moveSpeed);
                        isInAir = true;
                        animatorComp.applyRootMotion = false;
                    }
                }

                freeLook = CrossPlatformInputManager.GetButton("freeLook"); // we can still look around when flowing in time and space
            }

            // Move the player around the scene.
            if (isInAir)
                base.Fall();
            else
                base.Move(inputVelocity.x, inputVelocity.z, isRunning);

            // Turn the player to face the mouse cursor.
            base.Turning();

            // Animate the player.
            base.Animating(inputVelocity.x, inputVelocity.z);
        }
    }
}