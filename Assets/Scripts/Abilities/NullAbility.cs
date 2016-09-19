using System.Collections.Generic;
using Assets.Scripts.Entity;

namespace Assets.Scripts.Abilities
{
    class NullAbility : AbstractAbility
    {
        public override string name
        {
            get
            {
                return "NullAbility";
            }
        }

        public override void applyFailure(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
            UnityEngine.MonoBehaviour.print("Ability not assigned");
            Constants.chatManager.addLine("Ability not assigned");
        }

        public override void applySuccess(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
            UnityEngine.MonoBehaviour.print("Ability not assigned");
            Constants.chatManager.addLine("Ability not assigned");
        }

        public override bool canUse(int version, EntityController user, List<PraeObject> targets)
        {
            return false;
        }

        public override List<PraeObject> getTargets(int version, EntityController user)
        {
            return null;
        }

        public override int getTestModifier(int version, EntityController user, List<PraeObject> targets)
        {
            return 0;
        }

        public override void makeVisuals(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
        }

        public override void useOverride()
        {}
    }
}
