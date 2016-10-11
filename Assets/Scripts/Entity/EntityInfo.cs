using UnityEngine;
using System.Xml.Serialization;

namespace Assets.Scripts.Entity
{
    [System.Serializable]
    [XmlRoot("EntityInfo")]
    public class EntityInfo
    {
        public Attributes attr; // container for all basic 
        ECharClass _charClass;
        EAttributeGroup primaryAttr;
        EAttributeGroup secondaryAttr;

        public EntityInfo()
        {
            attr = new Attributes();
        }

        void Start()
        {
            // TODO: only for testing
            characterClass = ECharClass.IndustrieAdept;
        }


        public int getAttributeValue(EAttributeGroup A) { return attr.getAttributeLevel(A); }

        /**
         * some abilities or stats rely on the primary attribute
         */
        public int getPrimaryAttributeLevel() { return attr.getAttributeLevel(primaryAttr); }

        /**
         * some abilities or stats rely on the secondary attribute
         */
        public int getSecondaryAttributeLevel() { return attr.getAttributeLevel(secondaryAttr); }

        /**
         * XML INTERFACE
         ********************/

        public System.Collections.Generic.List<Managers.AttributeGroupSaveData> attrGrpList
        {
            get { return attr.attrGrpList; }
            set { attr.attrGrpList = value; }
        }

        public System.Collections.Generic.List<Managers.AttributeOtherSaveData> attrOtherList
        {
            get { return attr.attrOtherList; }
            set { attr.attrOtherList = value; }
        }

        /**
         * GETTER AND SETTER
         ********************/

        /**
         * load some basic presets based on the given character class
         */
        public ECharClass characterClass
        {
            get
            {
                return _charClass;
            }

            set
            {
                _charClass = value;
                switch (value)
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
                        throw new System.NotImplementedException();
                    default:
                        throw new System.NotImplementedException("new class not implemented?!");
                }

                int mana = attr.getAttributeLevel(primaryAttr) * 10 + attr.getAttributeLevel(secondaryAttr) * 5;

                attr.setAttributeTo(EAttributeOther.MaP, mana);
            }
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
                int SmP = attr.getAttributeLevel(EAttributeOther.SmP) + (int)System.Math.Ceiling(diff / 5f); // life to drain
                attr.setAttributeTo(EAttributeOther.SmP, SmP);
            }
        }

        public int MaP
        {
            get { return attr.getAttributeLevel(EAttributeOther.MaP); }

            set
            {
                if (value < 0)
                    Debug.LogError("Mapmax cannot be negative!");
                attr.setAttributeTo(EAttributeOther.MaP, value);
            }
        }

        public int MaPmax
        {
            get { return attr.getAttributeLevel(EAttributeOther.MaPmax); }

            set
            {
                if (value < 0)
                    Debug.LogError("Mapmax cannot be negative!");
                attr.setAttributeTo(EAttributeOther.MaPmax, value);
            }
        }
    }
}