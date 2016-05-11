using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Abilities
{
    abstract class AbstractAbility
    {
        protected EUsageMode usageMode = EUsageMode.none;
        protected PlayerController playerC;
        
        public AbstractAbility(PlayerController playerC)
        {
            this.playerC = playerC;
        }

        public abstract void use();
        public abstract bool canUse();

        public EUsageMode getUsageMode()
        {
            return usageMode;
        }
    }
}
