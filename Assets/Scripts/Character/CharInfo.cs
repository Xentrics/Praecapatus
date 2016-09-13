using System;
using UnityEngine;

namespace Assets.Scripts.Character
{
    class CharInfo : MonoBehaviour
    {
        public Attributes attr; // container for all basic 
        ECharClass charClass;
        EAttrGrp primaryAttr;
        EAttrGrp secondaryAttr;

        void Awake()
        {
            attr = GetComponent<Attributes>();
            if (attr == null)
            {
                attr = gameObject.AddComponent<Attributes>(); // may need some testing!
            }
        }

        void Start()
        {
            // TODO: only for testing
            setCharacterClass(ECharClass.IndustrieAdept);
        }

        public int getAttributeValue(EAttrGrp A)
        {
            return attr.getAttributeLevel(A);
        }

        /**
         * load some basic presets based on the given character class
         */
        public void setCharacterClass(ECharClass newClass)
        {
            switch(newClass)
            {
                case ECharClass.IndustrieAdept:
                    primaryAttr = EAttrGrp.IN;
                    secondaryAttr = EAttrGrp.LO;
                    break;
                case ECharClass.Technici:
                    primaryAttr = EAttrGrp.LO;
                    secondaryAttr = EAttrGrp.GE;
                    break;
                case ECharClass.Logenmagier:
                    throw new NotImplementedException();
                    break;
            }
        }

        /**
         * some abilities or stats rely on the primary attribute
         */
        public int getPrimaryAttributeLevel()
        {
            return attr.getAttributeLevel(primaryAttr);
        }

        /**
         * some abilities or stats rely on the secondary attribute
         */
        public int getSecondaryAttributeLevel()
        {
            return attr.getAttributeLevel(secondaryAttr);
        }
    }
}