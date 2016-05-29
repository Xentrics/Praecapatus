using Assets.Scripts.Abilities;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entity;

namespace Assets.Scripts.Managers
{
    class AbilityManager : MonoBehaviour
    {
        public static readonly int DEFAULT_ABILITY_LEVEL = 0;
        public static readonly int UNLEARNED = 0;

        PlayerController_Old playerC;
        Dictionary<EAbilities, AbstractAbility> abilities;

        public void Awake()
        {
            if (playerC == null)
                playerC = GetComponent<PlayerController_Old>();

            abilities = new Dictionary<EAbilities, AbstractAbility>();
            abilities.Add(EAbilities.null_, new NullAbility());
            abilities.Add(EAbilities.test, new TestAbility());
            // TODO: add abilities here
        }

        public AbstractAbility getAbility(EAbilities A)
        {
            return abilities[A];
        }
    }
}
