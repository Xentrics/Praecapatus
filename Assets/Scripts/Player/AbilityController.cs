using Assets.Scripts.Abilities;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Player
{
    class AbilityController : MonoBehaviour
    {
        public static readonly int DEFAULT_ABILITY_LEVEL = 0;
        public static readonly int UNLEARNED = -1;

        PlayerController playerC;
        Dictionary<EAbilities, AbstractAbility> abilities;

        public void Awake()
        {
            if (playerC == null)
                playerC = GetComponent<PlayerController>();
        }

        public void Start()
        {
            abilities = new Dictionary<EAbilities, AbstractAbility>();
            abilities.Add(EAbilities.test, new TestAbility());
            // TODO: add abilities here
        }
    }
}
