using Assets.Scripts.Exception;
using Assets.Scripts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    class AbilityTest : AbstractAbility
    {
        public AbilityTest()
        {
            this._attributeGrp = EAttributeGroup.KO;
            this._usageMode = EUsageMode.instant;
        }

        public override string name
        {
            get
            {
                return "TestAbility";
            }
        }

        public override void applyFailure(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
            UnityEngine.MonoBehaviour.print("Test ability finally failed!");
        }

        public override void applySuccess(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
            UnityEngine.MonoBehaviour.print("Test ability successfully used!");
        }

        public override bool canUse(int version, EntityController user, List<PraeObject> targets)
        {
            // no self cast
            return targets == null || !targets.Contains(user.praeObject);
        }

        public override List<PraeObject> getTargets(int version, EntityController user)
        {
            return null;
        }

        public override int getTestModifier(int version, EntityController user, List<PraeObject> targets)
        {
            if (targets == null || targets.Count() == 0)
                return 0;

            float dist = Vector3.Distance(user.position, targets.First().position);
            if (dist > user.praeObject.meleeRange)
                throw new GameLogicException("Melee attacks shall only be usable when user within melee range!");
            return 0;
        }

        public override void makeVisuals(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
            throw new NotImplementedException();
        }

        public override void useOverride()
        {
            throw new NotImplementedException();
        }
    }
}
