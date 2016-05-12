using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Player;

namespace Assets.Scripts.Abilities
{
    abstract class InstantUseAbility : AbstractAbility
    {
        public InstantUseAbility(PlayerController playerC) : base(playerC)
        {

        }
    }
}
