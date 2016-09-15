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
        public int fw = AbilityManager.UNLEARNED; // 'fertigkeitswert', the base ability level. This is used for dice-rolls

        protected EAttrGrp attributeGrp;
        protected EUsageMode usageMode = EUsageMode.none;
        protected bool useableWithoutLearning = true; // if level equals UNLEARNED, shall we still be able to make a test for this ability

        public abstract void useOverride();

        /**
         * test, whether or not this ability can be used by 'user' on 'targets'#
         * @user: the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract bool canUse(int version, Interactable user, List<Interactable> targets);

        /**
         * TODO: HEADER NEEDS SOME INPUT
         * the returned value will be added to the ability level pool (fw + aw + modifier)
         * @version: most abilities have different usage modes. Can be used as bitflag
         * @user: the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract int getTestModifier(int version, Interactable user, List<Interactable> targets);

        /**
         * gets called from the TestManager after rolling some dice
         * rp are the remaining points of the test
         * @version: most abilities have different usage modes. Can be used as bitflag
         * @rp: the absolute difference between the required and the roled test value
         * @luck:    normal, slipup or luck
         * @user: the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract void applySuccess(int version, int rp, ELuck luck, Interactable user, List<Interactable> targets);

        /**
         * TODO: method header may need some tweaking later on
         * is called from the ability manager, when this ability failed on use
         * @version: most abilities have different usage modes. Can be used as bitflag
         * @rp: the absolute difference between the required and the roled test value
         * @luck:    normal, slipup or luck
         * @user: the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract void applyFailure(int version, int rp, ELuck luck, Interactable user, List<Interactable> targets);

        /**
         * TODO:     method header may need some tweaking later on
         * func:     handles all the visual effects in the game
         * @version: most abilities have different usage modes. Can be used as bitflag
         * @rp:      the absolute difference between the required and the roled test value
         * @luck:    normal, slipup or luck
         * @user:    the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract void makeVisuals(int version, int rp, ELuck luck, Interactable user, List<Interactable> targets);

        /**
         * returns the main attribute which adds to the fw during the dice roll
         * see EAttrGrp for all attributes available
         */
        public EAttrGrp getAttributeGroup()
        {
            return attributeGrp;
        }

        /**
         * return the state the ability is currently used
         * see EUsageMode for possible states
         */
        public EUsageMode getUsageMode()
        {
            return usageMode;
        }

        public abstract string name
        {
            get;
        }

        public override string ToString()
        {
            return "Abi: " + this.name;
        }
    }
}
