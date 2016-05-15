using UnityEngine;

namespace Assets.Scripts.Character
{
    class CharInfo : MonoBehaviour
    {
        public CharAttributes attr;

        void Awake()
        {
            attr = GetComponent<CharAttributes>();
            if (attr == null)
            {
                attr = gameObject.AddComponent<CharAttributes>(); // may need some testing!
            }
        }

        public int getAttributeValue(EAttrGrp A)
        {
            return attr.getAttributeLevel(A);
        }
    }
}