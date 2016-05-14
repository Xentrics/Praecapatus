using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        public Camera followCamera;              // The default third person camera

        public float moveSpeed = 6f;            // The speed that the player will move at.
        public float turnSpeed = 0.25f;         // The rate at which the player can turn around (independent of the camera)
        public float jumpStrength = 12f;         // This values defines with respect to the characters weight how high he can jump
        public float gravityMultiplier = 4f;     // Additional gravity that can be applied

        public bool freeLook = false;           // If true, the rotation of the player will adjust to the cameras rotation. May be disabled by pressing/holding a certain key
        bool cinimaticModeOn = false;     // disable free movement etc. for cinematic actions

        Vector3 velocity;                        // The vector to store the direction of the player's movement.
        Vector3 inputVelocity;                   // The amount of movement caused by mouse/keyboard input
        Vector3 lookDir;                         // 

        float GroundCheckDistance = 0.1f;
        float OrigGroundCheckDistance;
        public Vector3 groundCheckOffset;


        Animator animatorComp;                   // Reference to the animator component.
        Rigidbody rigitBodyComp;                 // Reference to the player's rigidbody.
        int floorMask;                           // A layer mask so that a ray can be cast just at gameobjects on the floor layer.

        // character states
        public bool isInAir = false;                    // shall be true, if the player is in the air (for whatever reason)
        bool canJump = true;                     // can be false duo to exhaustion and other reasons
        bool isRunning = false;
        bool canRun = true;                      // can be false duo to exhaustion and other reasons

        void Awake()
        {
            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask("Floor");
            lookDir = new Vector3();
            velocity = new Vector3();
            inputVelocity = new Vector3();
            OrigGroundCheckDistance = GroundCheckDistance;

            // Set up references. They cannot be null since they are required!
            animatorComp = GetComponent<Animator>();
            rigitBodyComp = GetComponent<Rigidbody>();
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
                Fall();
            else
                Move(inputVelocity.x, inputVelocity.z, isRunning);

            // Turn the player to face the mouse cursor.
            Turning();

            // Animate the player.
            Animating(inputVelocity.x, inputVelocity.z);
        }

        void Fall()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rigitBodyComp.AddForce(extraGravityForce);

            GroundCheckDistance = rigitBodyComp.velocity.y < 0 ? OrigGroundCheckDistance : 0.01f;
        }


        void Move(float h, float v, bool run)
        {
            // Set the movement vector based on the axis input.
            velocity.Set(h, 0f, v);

            // Normalise the movement vector and make it proportional to the speed per second.
            velocity = velocity.normalized * moveSpeed * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            rigitBodyComp.MovePosition(transform.position + Quaternion.LookRotation(lookDir * turnSpeed) * velocity);
        }


        void Turning()
        {
            transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.LookRotation(lookDir), turnSpeed);
        }


        void Animating(float h, float v)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f || v != 0f;

            // Tell the animator whether or not the player is walking.
            animatorComp.SetBool("IsWalking", walking);
        }


        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (!isInAir && Time.deltaTime > 0)
            {
                Vector3 v = (animatorComp.deltaPosition * moveSpeed) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = rigitBodyComp.velocity.y;
                rigitBodyComp.velocity = v;
            }
        }


        /**
         * check if the player is still in the air using ray casts
         */
        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + groundCheckOffset + (Vector3.up * 0.1f), transform.position + groundCheckOffset + (Vector3.up * 0.1f) + (Vector3.down * GroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + groundCheckOffset + (Vector3.up * 0.1f), Vector3.down, out hitInfo, GroundCheckDistance))
            {
                // hit something.
                isInAir = false;
                animatorComp.applyRootMotion = true;
            }
            else
            {
                isInAir = true;
                animatorComp.applyRootMotion = false;
            }
        }

        public Vector3 getPosition()
        {
            return rigitBodyComp.position;
        }
    }
}