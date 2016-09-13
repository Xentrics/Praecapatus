using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Character
{
    /**
     * this class allows an easy way to set attribute using the unity editor while maining the clean code (using dictionaries)
     */
    class CharAttributeOverrider : MonoBehaviour
    {
        public static readonly int NO_OVERRIDE = -1;
        
        public int KO = NO_OVERRIDE;
        public int MO = NO_OVERRIDE;
        public int GE = NO_OVERRIDE;
        public int AU = NO_OVERRIDE;
        public int IN = NO_OVERRIDE;
        public int LO = NO_OVERRIDE;

        public Attributes attrToOverride;
        public Dictionary<EAttrGrp, int> attr_orig;
        

        public void Awake()
        {            
        }

        public void Start()
        {
            // get CharAttributes component if not defined in unity editor
            if (attrToOverride == null)
            {
                CharInfo info = GetComponent<CharInfo>();
                if (info != null)
                    attrToOverride = info.attr;
                else
                    attrToOverride = GetComponent<Attributes>();

                if (attrToOverride == null)
                    throw new NullReferenceException("CharAttributeOverrider: Could not aquire CharAttributes component!");
            }

            // store old values (just in case)
            attr_orig = attrToOverride.deepCopy(); // deep copy

            // apply override
            overrideAttributes();
        }

        /**
         * restore attribute values to their original values
         */
        public void resetToOldValues()
        {
            foreach (EAttrGrp A in Enum.GetValues(typeof(EAttrGrp)))
            {
                attrToOverride.setAttributeTo(A, attr_orig[A]);
            }
        }

        public void overrideAttributes()
        {
            if (AU != NO_OVERRIDE)
                attrToOverride.setAttributeTo(EAttrGrp.AU, AU);
            if (GE != NO_OVERRIDE)
                attrToOverride.setAttributeTo(EAttrGrp.GE, GE);
            if (IN != NO_OVERRIDE)
                attrToOverride.setAttributeTo(EAttrGrp.IN, IN);
            if (KO != NO_OVERRIDE)
                attrToOverride.setAttributeTo(EAttrGrp.KO, KO);
            if (LO != NO_OVERRIDE)
                attrToOverride.setAttributeTo(EAttrGrp.LO, LO);
            if (MO != NO_OVERRIDE)
                attrToOverride.setAttributeTo(EAttrGrp.MO, MO);
        }
    }
}
