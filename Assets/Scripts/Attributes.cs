using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts.Exception;

namespace Assets.Scripts
{
    /**
     * a component that contains all basic attributes and methods to modify them
     * may be attached to any character or entity
     */
    public class Attributes
    { 
        public static readonly int minLevel = 0;
        // these are only valid for attribute groups
        public static readonly int defLevel = 10;
        public static readonly int maxLevel = 20;

        // Container for all
        Dictionary<EAttributeGroup, int> attributeGroups;
        Dictionary<EAttributeOther, int> attributesOther;

        public Attributes()
        {
            attributeGroups = new Dictionary<EAttributeGroup, int>(Constants.NUM_ATTRIBUTE_GROUPS);
            foreach (EAttributeGroup A in Enum.GetValues(typeof(EAttributeGroup)))
                attributeGroups.Add(A, Attributes.defLevel);

            attributesOther = new Dictionary<EAttributeOther, int>(Constants.NUM_ATTRIBUTE_OTHER);
            foreach (EAttributeOther A in Enum.GetValues(typeof(EAttributeOther)))
                attributesOther.Add(A, 20);
        }

        /**
         * XML INTERFACE
         ********************/

        public List<Managers.AttributeGroupSaveData> attrGrpList
        {
            get
            {
                List<Managers.AttributeGroupSaveData> ret = new List<Managers.AttributeGroupSaveData>(attributeGroups.Count+1);
                int i = 0;
                foreach (EAttributeGroup a in attributeGroups.Keys)
                {
                    ret.Add(new Managers.AttributeGroupSaveData());
                    ret[i].attr = a;
                    ret[i].val = attributeGroups[a];
                    ++i;
                }

                return ret;
            }

            set
            { 
                foreach (Managers.AttributeGroupSaveData a in value)
                {
                    attributeGroups[a.attr] = a.val;
                }
            }
        }

        public List<Managers.AttributeOtherSaveData> attrOtherList
        {
            get
            {
                List<Managers.AttributeOtherSaveData> ret = new List<Managers.AttributeOtherSaveData>(attributeGroups.Count + 1);
                int i = 0;
                foreach (EAttributeOther a in attributesOther.Keys)
                {
                    ret.Add(new Managers.AttributeOtherSaveData());
                    ret[i].attr = a;
                    ret[i].val = attributesOther[a];
                    ++i;
                }

                return ret;
            }

            set
            {
                foreach (Managers.AttributeOtherSaveData a in value)
                {
                    attributesOther[a.attr] = a.val;
                }
            }
        }

        /**
         * GETTER AND SETTER
         ***********************/

        /*
         * change the level of the given attribute to 'value' for this instance
         */
        public void setAttributeTo(EAttributeOther A, int value)
        {
            if (value < minLevel)
                throw new ArgumentOutOfRangeException("You attempted to set an attribute to high or to low! Value: " + value);
            else
                attributesOther[A] = value; // update to new level
        }

        /*
         * change the level of the given attribute to 'value' for this instance
         */
        public void setAttributeTo(EAttributeGroup A, int value)
        {
            if (value < minLevel || value > maxLevel)
                throw new ArgumentOutOfRangeException("You attempted to set an attribute to high or to low! Value: " + value);
            else
                attributeGroups[A] = value; // update to new level
        }

        /**
         * Given A, increase the level of A by 'by'
         * return TRUE, if the given attribute A can be raised by 'by' levels
         */
        public void increaseAttribute(EAttributeGroup A, int by = 1)
        {
            if (canLevelUp(A, by))
            {
                attributeGroups[A] += by;
            }
            else
                throw new InvalidAttributeLevelException("increaseAttribute");
        }

        /**
         * return TRUE, if the given attribute A can be raised by 'by' levels
         */
        public bool canLevelUp(EAttributeGroup A, int by = 1)
        {
            return attributeGroups[A] + by <= maxLevel;
        }

        /**
         * given A, return the current level of A
         */
        public int getAttributeLevel(EAttributeGroup A)
        {
            return attributeGroups[A];
        }

        /**
         * given A, return the current level of A
         */
        public int getAttributeLevel(EAttributeOther A)
        {
            return attributesOther[A];
        }

        public Dictionary<EAttributeGroup, int> deepCopy()
        {
            return new Dictionary<EAttributeGroup, int>(attributeGroups);
        }

        /**
         * returns a string containing all attributes with their correspending level
         */
        public string toString()
        {
            string s = "";
            foreach (EAttributeGroup A in Enum.GetValues(typeof(EAttributeGroup)))
                s += A.ToString() + " [" + attributeGroups[A] + "]";
            return s;
        }
    }
}