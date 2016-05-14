using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Player;
using Assets.Scripts.Abilities;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(CharInfo))]
    class TestManager : MonoBehaviour
    {
        PlayerController playerC;
        CharInfo charInfo;

        public void Awake()
        {
            playerC = GetComponent<PlayerController>();
            charInfo = GetComponent<CharInfo>();
        }

        public void initiateNewTest(ETestMode mode)
        {
            switch (mode)
            {
                case ETestMode.instant:
                    break;
                case ETestMode.prolonged:
                    break;
                case ETestMode.custom:
                    break;
            }
        }

        /**
         * 
         */
        public void testInstant(int minRP, AbstractAbility ability, PlayerController user, List<PlayerController> targets)
        {
            if (ability.canUse(user, targets))
            {
                int c = ability.getTestModifier(user, targets);
                int aw = charInfo.getAttributeValue(ability.getAttributeGroup());
                int fw = ability.level;
                bool failed = false;

                int diceroll = UnityEngine.Random.Range(1, 20);
                int rp = fw + aw - diceroll;

                if (diceroll == 1)
                {
                    // test for critical "luck"
                    int second_roll = UnityEngine.Random.Range(1, 20);
                    if (second_roll <= aw)
                        rp += 20;
                }
                else if (diceroll == 21)
                {
                    // test for critical "failure"
                    int mod = Math.Max(0, fw + aw - 20);
                    int reroll = UnityEngine.Random.Range(1, 20);
                    do
                    {
                        if (reroll > aw + fw + mod)
                            failed = true; // Patzer
                    } while (!failed && reroll >= 19);
                }

                print("Dice: " + diceroll + " rp: " + rp + " fw: " + fw + " aw: " + aw);

                if (rp >= minRP)
                    ability.applySuccess(rp, targets);
                else
                    ability.applyFailure(rp, user, targets);
            }
            else
            {
                Debug.Log("Cannot use ability " + ability.ToString() + " yet.");
            }
        }

        private bool hasTestFinished()
        {
            throw new NotImplementedException();
        }

        private void useSuccessfulAbility(int by)
        {
            throw new NotImplementedException();
        }

        private void useFailedAbility(int by)
        {
            throw new NotImplementedException();
        }
    }
}
