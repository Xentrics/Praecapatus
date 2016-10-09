using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    static class Useful
    {
        // static constructor
        static Useful()
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

        public static readonly float boxTime = 5f;
        public static readonly Color boxColor = Color.red;
        public static void DrawDebugBox(Vector3 v3Center, Vector3 v3Extents)
        {
            Vector3 v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
            Vector3 v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
            Vector3 v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
            Vector3 v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
            Vector3 v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
            Vector3 v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
            Vector3 v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
            Vector3 v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

            Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, boxColor, boxTime);
            Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, boxColor, boxTime);
            Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, boxColor, boxTime);
            Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, boxColor, boxTime);

            Debug.DrawLine(v3BackTopLeft, v3BackTopRight, boxColor, boxTime);
            Debug.DrawLine(v3BackTopRight, v3BackBottomRight, boxColor, boxTime);
            Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, boxColor, boxTime);
            Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, boxColor, boxTime);

            Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, boxColor, boxTime);
            Debug.DrawLine(v3FrontTopRight, v3BackTopRight, boxColor, boxTime);
            Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, boxColor, boxTime);
            Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, boxColor, boxTime);
        }

        public static string Encrypt(string toEncrypt)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            // 256-AES key
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
            rDel.Padding = PaddingMode.PKCS7;
            // better lang support
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return System.Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string toDecrypt)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            // AES-256 key
            byte[] toEncryptArray = System.Convert.FromBase64String(toDecrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
            rDel.Padding = PaddingMode.PKCS7;
            // better lang support
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
