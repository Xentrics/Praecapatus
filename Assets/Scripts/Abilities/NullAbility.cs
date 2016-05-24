using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entity;

namespace Assets.Scripts.Abilities
{
    class NullAbility : AbstractAbility
    {
        public override void applyFailure(int rp, EntityController user, List<EntityController> targets)
        {
            UnityEngine.MonoBehaviour.print("Ability not assigned");
        }

        public override void applySuccess(int rp, List<EntityController> targets)
        {
            UnityEngine.MonoBehaviour.print("Ability not assigned");
        }

        public override bool canUse(EntityController user, List<EntityController> targets)
        {
            return false;
        }

        public override int getTestModifier(EntityController user, List<EntityController> targets)
        {
            return 0;
        }

        public override void useOverride()
        {}
    }
}
