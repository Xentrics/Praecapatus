﻿using Assets.Scripts.Exception;
using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    class TestAbility : AbstractAbility
    {
        public TestAbility()
        {
            this.attributeGrp = EAttrGrp.KO;
            this.usageMode = EUsageMode.instant;
        }

        public override void applyFailure(int rp, PlayerController user, List<PlayerController> targets)
        {
            UnityEngine.MonoBehaviour.print("Test ability finally failed!");
        }

        public override void applySuccess(int rp, List<PlayerController> targets)
        {
            UnityEngine.MonoBehaviour.print("Test ability successfully used!");
        }

        public override bool canUse(PlayerController user, List<PlayerController> targets)
        {
            // no self cast
            return targets == null || !targets.Contains(user);
        }

        public override int getTestModifier(PlayerController user, List<PlayerController> targets)
        {
            if (targets == null || targets.Count() == 0)
                return 0;

            float dist = Vector3.Distance(user.getPosition(), targets.First().getPosition());
            if (dist > user.getMeleeRange())
                throw new GameLogicException("Melee attacks shall only be usable when user within melee range!");
            return 0;
        }

        public override void useOverride()
        {
            throw new NotImplementedException();
        }
    }
}