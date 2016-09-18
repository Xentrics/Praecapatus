using System;
using UnityEngine;

namespace Assets.Scripts.Entity
{
    class EntityInfo : MonoBehaviour
    {
        public Attributes attr; // container for all basic 
        ECharClass charClass;
        EAttributeGroup primaryAttr;
        EAttributeGroup secondaryAttr;

        void Awake()
        {
            attr = GetComponent<Attributes>();
            if (attr == null)
                attr = gameObject.AddComponent<Attributes>();
        }

        void Start()
        {
            // TODO: only for testing
            setCharacterClass(ECharClass.IndustrieAdept);
        }


        public int getAttributeValue(EAttributeGroup A)
        {
            return attr.getAttributeLevel(A);
        }

        /**
         * load some basic presets based on the given character class
         */
        public void setCharacterClass(ECharClass newClass)
        {
            charClass = newClass;
            switch (newClass)
            {
                case ECharClass.IndustrieAdept:
                    primaryAttr = EAttributeGroup.IN;
                    secondaryAttr = EAttributeGroup.LO;
                    break;
                case ECharClass.Technici:
                    primaryAttr = EAttributeGroup.LO;
                    secondaryAttr = EAttributeGroup.GE;
                    break;
                case ECharClass.Logenmagier:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException("new class not implemented?!");
            }

            int mana = attr.getAttributeLevel(primaryAttr) * 10 + attr.getAttributeLevel(secondaryAttr) * 5;
            attr.setAttributeTo(EAttributeOther.MaP, mana);
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

        /**
         * @by: the absolute amount of mana to drain from this entity
         */
        public void drainMaP(int by)
        {
            if (attr.getAttributeLevel(EAttributeOther.MaP) >= by)
            {
                int diff = attr.getAttributeLevel(EAttributeOther.MaP) - by;
                attr.setAttributeTo(EAttributeOther.MaP, diff);
            }
            else
            {
                // not enough mana left. Drain life
                int diff = by - attr.getAttributeLevel(EAttributeOther.MaP); // remaining mana to drain from life
                attr.setAttributeTo(EAttributeOther.MaP, 0);
                int SmP = attr.getAttributeLevel(EAttributeOther.SmP) + (int)Math.Ceiling(diff / 5f); // life to drain
                attr.setAttributeTo(EAttributeOther.SmP, SmP);
            }
        }

        public int MaP
        {
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Mapmax cannot be negative!");
                else
                    attr.setAttributeTo(EAttributeOther.MaP, value);
            }

            get
            {
                return attr.getAttributeLevel(EAttributeOther.MaP);
            }
        }

        public int MaPmax
        {
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Mapmax cannot be negative!");
                else
                    attr.setAttributeTo(EAttributeOther.MaPmax, value);
            }

            get
            {
                return attr.getAttributeLevel(EAttributeOther.MaPmax);
            }
        }
    }
}