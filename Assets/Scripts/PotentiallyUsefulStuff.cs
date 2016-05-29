using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    class PotentiallyUsefulStuff
    {
        /**
         * make animation fróm a list of sprites
         */
        private AnimationClip CreateSpriteAnimationClip(string name, List<Sprite> sprites, int fps, bool raiseEvent = false)
        {
            int framecount = sprites.Count;
            float frameLength = 1f / 30f;

            AnimationClip clip = new AnimationClip();
            clip.frameRate = fps;

            AnimationUtility.GetAnimationClipSettings(clip).loopTime = true;

            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(SpriteRenderer);
            curveBinding.propertyName = "m_Sprite";

            // set animation keys
            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[framecount];

            for (int i = 0; i < framecount; i++)
            {
                ObjectReferenceKeyframe kf = new ObjectReferenceKeyframe();
                kf.time = i * frameLength;
                kf.value = sprites[i];
                keyFrames[i] = kf;
            }

            clip.name = name;

            //AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Generic);
            //if (name != "Fall")
            Debug.Log(clip.wrapMode);
            clip.wrapMode = WrapMode.Once;
            //setAnimationLoop(clip);
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

            //clip.ValidateIfRetargetable(true);

            if (raiseEvent)
            {
                //AnimationUtility.SetAnimationEvents(clip, new[] { new AnimationEvent() { time = clip.length, functionName = "on" + name } });
            }
            //clip.AddEvent(e);
            return clip;
        }
    }
}
