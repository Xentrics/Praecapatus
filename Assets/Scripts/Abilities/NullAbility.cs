using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entity;

namespace Assets.Scripts.Abilities
{
    class NullAbility : AbstractAbility
    {
        public override void applyFailure(int version, int rp, EntityController user, List<EntityController> targets)
        {
            UnityEngine.MonoBehaviour.print("Ability not assigned");
        }

        public override void applySuccess(int version, int rp, List<EntityController> targets)
        {
            UnityEngine.MonoBehaviour.print("Ability not assigned");
        }

        public override bool canUse(int version, EntityController user, List<EntityController> targets)
        {
            return false;
        }

        public override int getTestModifier(int version, EntityController user, List<EntityController> targets)
        {
            return 0;
        }

        public override void makeVisuals(int version, int rp, EntityController user, List<EntityController> targets)
        {
            throw new NotImplementedException();
        }

        public override void useOverride()
        {}
    }
}
