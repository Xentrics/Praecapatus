using System;
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
        protected Vector3 _lookDir;                        // sprite look direction. This is independent of the actual transform.rotation
        protected Quaternion mapRotation;

        public float GroundCheckDistance = 0.1f;
        protected float OrigGroundCheckDistance;
        public Vector3 groundCheckOffset;

        protected Animator animComp;
        protected Rigidbody rigidBodyComp;                 // Reference to the player's rigidbody.
        protected SpriteRenderer spriteRenderer;           // lets us flit stuff easily
        protected Collider mainCollider;
        protected int floorMask;                           // A layer mask so that a ray can be cast just at gameobjects on the floor layer.

        // character states
        public bool isInAir = false;                       // shall be true, if the player is in the air (for whatever reason)
        protected bool canJump = true;                     // can be false duo to exhaustion and other reasons
        protected bool _bRun = true;
        protected bool canRun = true;                      // can be false duo to exhaustion and other reasons

        virtual protected void Awake()
        {
            mapRotation = Constants.gameLogic.worldViewRotation; // grab map rotation and never, ever change it!

            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask("Floor");
            _lookDir = new Vector3(1,0,0);
            velocity = new Vector3();
            inputVelocity = new Vector3();
            OrigGroundCheckDistance = GroundCheckDistance;

            // Set up references. They cannot be null since they are required!
            rigidBodyComp = GetComponent<Rigidbody>();
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
                Move(inputVelocity.x, inputVelocity.z, _bRun);

            // Turn the player to face the mouse cursor.
            //Turning();
        }

        void Update()
        {
            // Animate the player.
            Animating(inputVelocity.x, inputVelocity.z);
        }

        protected void Fall()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rigidBodyComp.AddForce(extraGravityForce);
        }


        protected void Move(float h, float v, bool run)
        {
            // set correct look direction
            // NOTE: because of sprites, the player can only look up/down/east or west, even if the is moving diagonal!
            // NOTE: therefor, horizontal animation is ALWAYS dominant during diagonal movement
            // NOTE: direction does not change while standing still
            if (h != 0)
                _lookDir.Set(Math.Sign(h), 0, 0);
            else if (v != 0)
                _lookDir.Set(0, 0, Math.Sign(v));


            // Set the movement vector based on the axis input.
            velocity.Set(h, 0f, v);

            // Normalise the movement vector and make it proportional to the speed per second.
            velocity = velocity.normalized * ((_bRun) ? runSpeed : moveSpeed) * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            rigidBodyComp.MovePosition(transform.position + mapRotation * velocity); // Quaternion.LookRotation(mapRotation * turnSpeed) * velocity
        }


        protected void Turning()
        {
            // transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.LookRotation(_lookDir), turnSpeed);
            // NOTE: should the sprite be rotated here? Better to that from the gamelogic...
            print("Why do you try turning?");
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
            else if (v != 0)
            {
                //animComp.SetInteger("direction", (v < 0) ? (int)EDirection.SOUTH : (int)EDirection.NORTH); // TODO: not yet implemented!
            }

            if (walking)
            {
                if (_bRun)
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
        protected void CheckGroundStatus(bool keepVelocity = false)
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + groundCheckOffset + (Vector3.up * 0.1f), transform.position + groundCheckOffset + (Vector3.up * 0.1f) + (Vector3.down * GroundCheckDistance), Color.red, 1f);
#endif
            // 0.1f is a small offset to start the ray from inside the character
            if (Physics.Raycast(transform.position + groundCheckOffset + (Vector3.up * 0.1f), Vector3.down, out hitInfo, GroundCheckDistance))
            {
                // hit something.
                isInAir = false;                            // allow input again
                animComp.SetBool("grounded", true);
                if (!keepVelocity)
                    rigidBodyComp.velocity = Vector3.zero;  // prevent sliding from velocity

                //rigidBodyComp.MovePosition(hitInfo.point);  // move player directly onto hit location
            }
            else
            {
                isInAir = true;                             // prevent movement input from player
                animComp.SetBool("grounded", false);
            }
        }


        /*******
         * GETTER AND SETTER
         ***************************/

        public void toggleRunning()
        {
            _bRun = !_bRun;
        }

        public bool running
        {
            get
            {
                return _bRun;
            }
            
            set
            {
                _bRun = value;
            }
        }

        public Vector3 position
        {
            get
            {
                return rigidBodyComp.position;
            }
        }

        public Vector3 lookDir
        {
            get
            {
                return _lookDir;
            }
        }
    }
}
