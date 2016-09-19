using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    static class PotentiallyUsefulStuff
    {
        // static constructor
        static PotentiallyUsefulStuff()
        { }

        /**
         * func: create a random point within a ring at high y=0
         * func: all point with be created, so that rin < sqrt(x^2+y^2) < rout >
         * @rin: inner radius (without point)
         * @rout: outer radius
         */
        public static Vector3 RandomPointInRing(float rin, float rout)
        {
            float x = Random.Range(-rout, rout);
            float z;
            if (Mathf.Abs(x) <= rin)
            {
                float h = Mathf.Sqrt(rin * rin - x * x);
                z = Random.Range(h, h + (rout - rin)) * (Random.Range(0, 2) * 2 - 1); // multiply with random sign
            }
            else
            {
                float h = Mathf.Sqrt(rout * rout - x * x);
                z = Random.Range(0, h) * (Random.Range(0, 2) * 2 - 1); // multiply with random sign
            }

            return new Vector3(x, 0, z);
        }

        /**
         * func: create n random points within a ring at high y=0
         * func: all point with be created, so that rin < sqrt(x^2+y^2) < rout >
         * @n: number of points to generate
         * @rin: inner radius (without points)
         * @rout: outer radius
         */
        public static Vector3[] RandomPointsInRing(int n, float rin, float rout)
        {
            Vector3[] points = new Vector3[n];
            for (int i=0; i<n; ++i)
                points[i] = RandomPointInRing(rin, rout);

            return points;
        }

        /**
         * make animation fróm a list of sprites
         */
        public static AnimationClip CreateSpriteAnimationClip(string name, List<Sprite> sprites, int fps, bool raiseEvent = false)
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
