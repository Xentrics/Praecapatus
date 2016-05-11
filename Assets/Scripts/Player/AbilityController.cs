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
            abilities.Add(EAbilities.test, new TestAbility(playerC));
            // TODO: add abilities here
        }
    }
}
