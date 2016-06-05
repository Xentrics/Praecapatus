using Assets.Scripts.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MultiSpriteAnimator : MonoBehaviour
    {
        SpriteRenderer spRenderer;
        public int framesPerSecond = 16; // will be overriden by each of the different sprite animations whenever we select a different one

        public SpriteAnimation IdleAnim;
        public SpriteAnimation WalkAnim;
        public SpriteAnimation RunAnim;
        public SpriteAnimation JumpUpAnim;
        public SpriteAnimation JumpForwAnim;
        public SpriteAnimation JumpBackAnim;

        public bool lookLeft = false;
        protected SpriteAnimation currentAnim;
        protected EEntityState currentState;
        protected int animIndex = 0;

        protected float lastFrameUpdateTime; // remember the last time at which we changed

        protected bool bHasPendingAnimation = false; // TRUE, if we wait for the old animation to finish
        protected SpriteAnimation pendingAnim;

        void Awake()
        {
            spRenderer = GetComponent<SpriteRenderer>();
            if (IdleAnim == null) IdleAnim = this.gameObject.AddComponent<SpriteAnimation>();
            if (WalkAnim == null) WalkAnim = this.gameObject.AddComponent<SpriteAnimation>();
            if (RunAnim == null)  RunAnim = this.gameObject.AddComponent<SpriteAnimation>();
            if (IdleAnim == null) JumpUpAnim = this.gameObject.AddComponent<SpriteAnimation>();
            if (IdleAnim == null) JumpForwAnim = this.gameObject.AddComponent<SpriteAnimation>();
            if (IdleAnim == null) JumpBackAnim = this.gameObject.AddComponent<SpriteAnimation>();
        }


        void Start()
        {
            currentAnim = IdleAnim;
            spRenderer.flipX = lookLeft;
            spRenderer.sprite = currentAnim.startAnimation();
            framesPerSecond = currentAnim.framesPerSecond;
            lastFrameUpdateTime = Time.time;
        }


        void Update()
        {
            print(currentState);
            if ((Time.time - lastFrameUpdateTime) * framesPerSecond >= 1.0f)
            {
                if (bHasPendingAnimation && currentAnim.hasFinishedEndAnimation())
                {
                    print("switch to pending anim");
                    Debug.Assert(pendingAnim != null, "Animator is pending, but has not pending animation set!");
                    bHasPendingAnimation = false;
                    currentAnim = pendingAnim;
                    pendingAnim = null; // clear, to be sure
                    currentAnim.startAnimation();
                    framesPerSecond = currentAnim.framesPerSecond;
                }
                // get the next frame
                lastFrameUpdateTime = Time.time;
                spRenderer.flipX = lookLeft;
                spRenderer.sprite = currentAnim.getNextFrame();
            }
        }


        public void setLookDirection(bool left)
        {
            lookLeft = left;
        }
    

        /*
         * returns "true", if the animation was changed
         * returns "false", if the animation is already playing or not set
         */
        public virtual bool setCurrentAnimation(EEntityState newState, bool interrupt)
        {
            if (currentState == newState)
                return false;
            else
                currentState = newState;

            if (interrupt)
            {
                print("anim interrupted");
                // clear pending anim, if set before
                bHasPendingAnimation = false;
                pendingAnim = null;
                // switch to new animation directly
                currentAnim = getSpriteAnimation(newState);
                currentAnim.startAnimation();
                framesPerSecond = currentAnim.framesPerSecond;
            }
            else
            {
                // check, if we already have a pending animation
                // in this case, we do not need to call "end animation" again and just override pendingAnim
                if (!bHasPendingAnimation)
                {
                    print("made Pending");
                    bHasPendingAnimation = true;
                    currentAnim.endAnimation();
                }

                // remember the next animation
                pendingAnim = getSpriteAnimation(newState);
            }

            return true;
        }


        public SpriteAnimation getSpriteAnimation(EEntityState state)
        {
            switch (state)
            {
                case EEntityState.idle: return IdleAnim;
                case EEntityState.walking: return WalkAnim;
                case EEntityState.running: return RunAnim;
                case EEntityState.jumpUp: return JumpUpAnim;
                case EEntityState.jumpForward: return JumpForwAnim;
                case EEntityState.jumpBackward: return JumpBackAnim;
                default:
                    print("Could not set anim for state " + state + " : Did you forget that entry?");
                    return null;
            }
        }
    }
}
