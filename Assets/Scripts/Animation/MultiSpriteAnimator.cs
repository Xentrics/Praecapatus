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
        public int framesPerSecond = 24;

        public Sprite[] IdleSprites;
        public Sprite[] WalkSprites;
        public Sprite[] RunSprites;

        public bool lookLeft = false;
        protected Sprite[] currentAnim;
        protected EEntityState currentState;
        protected int animIndex = 0;
        protected float animStartTime;

        void Awake()
        {
            spRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            currentAnim = IdleSprites;
            animStartTime = Time.time;
        }

        void Update()
        {
            animIndex = (int)( (Time.time - animStartTime) * framesPerSecond);
            animIndex %= currentAnim.Length;
            spRenderer.flipX = lookLeft;
            spRenderer.sprite = currentAnim[animIndex];
        }

        public void setLookDirection(bool left)
        {
            lookLeft = left;
        }
    
        /*
         * returns "true", if the animation was changed
         * returns "false", if the animation is already playing or not set
         */
        public virtual bool setCurrentAnimation(EEntityState state)
        {
            if (currentState == state)
                return false;
            else
                currentState = state;

            switch (state)
            {
                case EEntityState.idle: currentAnim = IdleSprites;
                    break;
                case EEntityState.walking: currentAnim = WalkSprites;
                    break;
                case EEntityState.running: currentAnim = RunSprites;
                    break;
                default:
                    print("Could not set anim for state " + state + " : Did you forget that entry?");
                    return false;
            }

            animIndex = 0;
            animStartTime = Time.time;
            return true;
        }

        public Sprite[] getSpriteAnimation(EEntityState state)
        {
            switch (state)
            {
                case EEntityState.idle: return IdleSprites;
                case EEntityState.walking: return WalkSprites;
                case EEntityState.running: return RunSprites;
                default:
                    return null;
            }
        }
    }
}
