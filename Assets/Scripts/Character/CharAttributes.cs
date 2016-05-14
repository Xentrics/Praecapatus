using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts.Exception;

/**
 * a component that contains all basic attributes and methods to modify them
 * may be attached to any character
 */
namespace Assets.Scripts
{
    class CharAttributes : MonoBehaviour
    {
        public static readonly int minLevel = 0;
        public static readonly int defLevel = 10;
        public static readonly int maxLevel = 20;

        Dictionary<EAttrGrp, int> attributes; // TODO: make private after testing

        void Awake()
        {
            // replaces the constructor
            attributes = new Dictionary<EAttrGrp, int>(Constants.NUM_ATTRIBUTES);
            foreach (EAttrGrp A in Enum.GetValues(typeof(EAttrGrp)))
            {
                attributes.Add(A, CharAttributes.defLevel);
            }
        }

        /*
         * change the level of the given attribute to 'value' for this instance
         */
        public void setAttributeTo(EAttrGrp A, int value)
        {
            if (value < minLevel || value > maxLevel)
                throw new ArgumentOutOfRangeException("You attempted to set an attribute to high or to low! Value: " + value);
            else
                attributes[A] = value; // update to new level
        }

        public void increaseAttribute(EAttrGrp A, int by = 1)
        {
            if (canLevelUp(A, by))
            {
                attributes[A] += by;
            }
            else
                throw new InvalidAttributeLevelException("increaseAttribute");
        }

        /**
         * return TRUE, if the given attribute A can be raised by 'by' levels
         */
        public bool canLevelUp(EAttrGrp A, int by = 1)
        {
            return attributes[A] + by <= maxLevel;
        }

        public int getAttributeLevel(EAttrGrp A)
        {
            return attributes[A];
        }

        public Dictionary<EAttrGrp, int> deepCopy()
        {
            return new Dictionary<EAttrGrp, int>(attributes);
        }

        /**
         * returns a string containing all attributes with their correspending level
         */
        public string toString()
        {
            string s = "";
            foreach (EAttrGrp A in Enum.GetValues(typeof(EAttrGrp)))
                s += A.ToString() + " [" + attributes[A] + "]";

            return s;
        }
    }
}