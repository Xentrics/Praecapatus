using Assets.Scripts.Entity;
using Assets.Scripts.Managers;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Scripts.Abilities
{
    [System.Serializable]
    public abstract class AbstractAbility
    {
        [XmlAttribute]
        public int fw = AbilityManager.UNLEARNED; // 'fertigkeitswert', the base ability level. This is used for dice-rolls

        protected EAttributeGroup _attributeGrp;
        protected EUsageMode _usageMode = EUsageMode.none;
        protected bool useableWithoutLearning = true; // if level equals UNLEARNED, shall we still be able to make a test for this ability

        public abstract void useOverride();

        /**
         * func: return a list of targets, if possible
         * func: use 'canUse' of targets before using the ability
         * @version: ability specific modifier
         * @user:    the one using the ability
         */
        public abstract List<PraeObject> getTargets(int version, EntityController user);

        /**
         * test, whether or not this ability can be used by 'user' on 'targets'
         * @user: the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract bool canUse(int version, EntityController user, List<PraeObject> targets);

        /**
         * TODO: HEADER NEEDS SOME INPUT
         * the returned value will be added to the ability level pool (fw + aw + modifier)
         * @version: most abilities have different usage modes. Can be used as bitflag
         * @user: the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract int getTestModifier(int version, EntityController user, List<PraeObject> targets);

        /**
         * gets called from the TestManager after rolling some dice
         * rp are the remaining points of the test
         * @version: most abilities have different usage modes. Can be used as bitflag
         * @rp: the absolute difference between the required and the roled test value
         * @luck:    normal, slipup or luck
         * @user: the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract void applySuccess(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets);

        /**
         * is called from the ability manager, when this ability failed on use
         * @version: most abilities have different usage modes. Can be used as bitflag
         * @rp: the absolute difference between the required and the roled test value
         * @luck:    normal, slipup or luck
         * @user: the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract void applyFailure(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets);

        /**
         * func:     handles all the visual effects in the game
         * @version: most abilities have different usage modes. Can be used as bitflag
         * @rp:      the absolute difference between the required and the roled test value
         * @luck:    normal, slipup or luck
         * @user:    the one using this ability
         * @targets: any kind of object/entity that is effected by this ability
         */
        public abstract void makeVisuals(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets);

        /**
         * returns the main attribute which adds to the fw during the dice roll
         * see EAttrGrp for all attributes available
         */
        [XmlAttribute("attrGrp")]
        public EAttributeGroup attributeGroup
        {
            get { return _attributeGrp; }
            protected set { _attributeGrp = value; }
        }

        /**
         * return the state the ability is currently used
         * see EUsageMode for possible states
         */
        [XmlAttribute("mode")]
        public EUsageMode usageMode
        {
            get { return _usageMode; }
            set { _usageMode = value; }
        }

        public abstract string name
        {
            get;
        }

        public override string ToString()
        {
            return "Abi: " + name;
        }
    }
}
