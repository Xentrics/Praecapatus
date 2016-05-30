using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class SpriteAnimation : MonoBehaviour
    {
        public EEntityState AnimType = EEntityState.custom;

        public int framesPerSecond = 16; // the frame rate at which to play this animation

        public Sprite[] startSprites = null;   // leave empty, if no custom start frames necessary
        public Sprite[] ongoingSprites = null; // the ongoing frames come here
        public Sprite[] endSprites = null;     // if animation can be eneded, this comes here

        Sprite[] curAnim;
        bool isStartAnim = true;

        public bool bLoopAnim = true;
        public bool bInteruptable = true;

        protected int animIndex = 0;

        void Start()
        {

        }

        /*
         * this should be called whenever this animation should start, regardless of it having a custom start
         */
        public Sprite startAnimation()
        {
            // get next animation
            if (startSprites == null || startSprites.Length == 0)
            {
                isStartAnim = false;
                curAnim = ongoingSprites;
            }
            else
            {
                curAnim = startSprites;
            }

            animIndex = 0;
            return null;
        }

        /*
         * this should be called if an animation should not be interrupted
         * or is "not interuptable"
         */
        public Sprite endAnimation()
        {

            if (endSprites != null || endSprites.Length == 0)
            {
                animIndex = -1;
                curAnim = endSprites;
            }

            return null;
        }

        /*
         * get current frame without increasing the index
         */
        public Sprite getCurrentFrame()
        {
            return curAnim[(animIndex >= 0) ? animIndex : 0];
        }

        /*
         * get current frame & increase the index
         */
        public Sprite getNextFrame()
        {
            animIndex += 1;
            if (isStartAnim && animIndex >=  startSprites.Length) // falls wir die start-anim beendet haben, nutzen wir die ongoing anim
            {
                isStartAnim = false;
                animIndex = 0; // wird beim index update um 1 erhöht, also auf 0
                curAnim = ongoingSprites;
            }

            if (bLoopAnim)
                animIndex %= curAnim.Length; // loop
            else
                animIndex = Math.Min(animIndex, curAnim.Length); // stop at last frame

            return curAnim[animIndex];
        }

        public virtual bool hasFinishedEndAnimation()
        {
            // no end animation or last frame reached
            return (endSprites == null || animIndex == endSprites.Length-1); 
        }
    }
}
