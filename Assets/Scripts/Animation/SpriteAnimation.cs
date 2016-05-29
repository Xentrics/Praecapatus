using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    class SpriteAnimation
    {
        Sprite[] startSprites; // leave empty, if no custom start frames necessary
        Sprite[] ongoingSprites;      // the ongoing frames come here
        Sprite[] endSprites;   // if animation can be eneded, this comes here

        Sprite[] curAnim;
        bool isStartAnim = true;

        public bool bLoopAnim = true;
        public bool bInteruptable = true;

        protected int animIndex = 0;

        /*
         * this should be called whenever this animation should start, regardless of it having a custom start
         */
        public Sprite startAnimation()
        {
            animIndex = 0;

            if (startSprites == null)
            {
                isStartAnim = false;
                curAnim = ongoingSprites;
            }
            else
            {
                curAnim = startSprites;
            }

            return getCurrentFrame();
        }

        /*
         * this should be called if an animation should not be interrupted
         * or is "not interuptable"
         */
        public Sprite endAnimation()
        {
            if (endSprites != null)
            {
                animIndex = 0;
                curAnim = endSprites;
                return getCurrentFrame();
            }
            else
            {
                return getNextFrame();
            }
            
        }

        /*
         * get current frame without increasing the index
         */
        public Sprite getCurrentFrame()
        {
            return curAnim[animIndex];
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
