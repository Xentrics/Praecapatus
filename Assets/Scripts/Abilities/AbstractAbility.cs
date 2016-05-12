using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Abilities
{
    abstract class AbstractAbility
    {
        public int level = AbilityController.UNLEARNED; // the base ability level. This is used for dice-rolls

        protected EUsageMode usageMode = EUsageMode.none;
        protected PlayerController playerC;
        
        public AbstractAbility(PlayerController playerC)
        {
            this.playerC = playerC;
        }

        public void use()
        {
            if (canUse())
                useOverride();
            else
                return;
        }

        public abstract void useOverride();

        public abstract bool canUse();

        public EUsageMode getUsageMode()
        {
            return usageMode;
        }
    }
}
