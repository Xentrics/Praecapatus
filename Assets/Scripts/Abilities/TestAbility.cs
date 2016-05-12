using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Abilities
{
    class TestAbility : AbstractAbility
    {
        public TestAbility(PlayerController playerC) : base(playerC)
        {
            usageMode = EUsageMode.instant;
        }

        public override bool canUse()
        {
            return true;
        }

        public override void useOverride()
        {
            throw new NotImplementedException();
        }
    }
}
