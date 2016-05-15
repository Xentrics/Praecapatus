using Assets.Scripts.Entity;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Abilities
{
    abstract class AbstractAbility
    {
        public int level = AbilityManager.UNLEARNED; // the base ability level. This is used for dice-rolls
        protected EAttrGrp attributeGrp;
        protected EUsageMode usageMode = EUsageMode.none;
        protected bool useableWithoutLearning = true; // if level equals UNLEARNED, shall we still be able to make a test for this ability

        public abstract void useOverride();

        /**
         * test, whether or not this ability can be used by 'user' on 'targets'
         */
        public abstract bool canUse(EntityController user, List<EntityController> targets);

        /**
         * TODO: HEADER NEEDS SOME INPUT
         * the returned value will be added to the ability level pool (fw + aw + modifier)
         */
        public abstract int getTestModifier(EntityController user, List<EntityController> targets);

        /**
         * gets called from the TestManager after rolling some dice
         * rp are the remaining points of the test
         */
        public abstract void applySuccess(int rp, List<EntityController> targets);

        /**
         * method header may need some tweaking later on
         */
        public abstract void applyFailure(int rp, EntityController user, List<EntityController> targets);

        public EAttrGrp getAttributeGroup()
        {
            return attributeGrp;
        }

        public EUsageMode getUsageMode()
        {
            return usageMode;
        }
    }
}
