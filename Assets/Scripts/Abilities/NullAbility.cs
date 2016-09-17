using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override void applyFailure(int version, int rp, ELuck luck, PraeObject user, List<PraeObject> targets)
        {
            UnityEngine.MonoBehaviour.print("Ability not assigned");
        }

        public override void applySuccess(int version, int rp, ELuck luck, PraeObject user, List<PraeObject> targets)
        {
            UnityEngine.MonoBehaviour.print("Ability not assigned");
        }

        public override bool canUse(int version, PraeObject user, List<PraeObject> targets)
        {
            return false;
        }

        public override int getTestModifier(int version, PraeObject user, List<PraeObject> targets)
        {
            return 0;
        }

        public override void makeVisuals(int version, int rp, ELuck luck, PraeObject user, List<PraeObject> targets)
        {
            throw new NotImplementedException();
        }

        public override void useOverride()
        {}
    }
}
