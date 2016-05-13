using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            UnityEngine.MonoBehaviour.print("Test ability successfully used!");
        }

        public override void applySuccess(int rp, List<PlayerController> targets)
        {
            UnityEngine.MonoBehaviour.print("Test ability finally failed!");
        }

        public override bool canUse(PlayerController user, List<PlayerController> targets)
        {
            return true;
        }

        public override int getTestModifier()
        {
            return 0;
        }

        public override void useOverride()
        {
            throw new NotImplementedException();
        }
    }
}
