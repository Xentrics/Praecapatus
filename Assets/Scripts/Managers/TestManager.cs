using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entity;
using Assets.Scripts.Abilities;
using Assets.Scripts.Character;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(CharInfo))]
    class TestManager : MonoBehaviour
    {
        Interactable playerC;
        CharInfo charInfo;
        ChatManager chatManager;
        GameManager gameManager;

        public void Awake()
        {
            playerC = GetComponent<PlayerController>();
            charInfo = GetComponent<CharInfo>();
        }

        public void Start()
        {
            chatManager = GameObject.FindGameObjectWithTag("ChatBox").GetComponent<ChatManager>();
            gameManager = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameManager>();
        }

        /**
         * must be called before making any dice roll testing
         */
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

        /*
         * Func: Basic dice roll test function for the use of abilities in the game
         * Func: this version should be called by entity controller
         * Func: uses GameManager to determine minimal required RP
         * @version: changes specifics of 'ability'
         * @minRP: minimum Restpunkte required for a successful test
         * @ability: the one that will be executed
         * @user: entity using the ability
         * @targets: potential targets for the ability (if necessary)
         */
        public void testInstant(int version, AbstractAbility ability, Interactable user, List<Interactable> targets)
        {
            int minRP = 0; //TODO: the GameManager should somehow determine this value
            this.testInstant(version, minRP, ability, user, targets);
        }

        /**
         * DEBUG VERSION
         * Func: Basic dice roll test function for the use of abilities in the game
         * @version: changes specifics of 'ability'
         * @minRP: minimum Restpunkte required for a successful test
         * @ability: the one that will be executed
         * @user: entity using the ability
         * @targets: potential targets for the ability (if necessary)
         */
        public void testInstant(int version, int minRP, AbstractAbility ability, Interactable user, List<Interactable> targets)
        {
            if (ability.canUse(version, user, targets))
            {
                int c = ability.getTestModifier(version, user, targets);
                int aw = charInfo.getAttributeValue(ability.getAttributeGroup());
                int fw = ability.fw;
                bool failed = false;

                int diceroll = UnityEngine.Random.Range(1, 20);
                int rp = fw + aw - c - diceroll;

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

                print("Ability: " + ability + "| Dice: " + diceroll + " rp: " + rp + " fw: " + fw + " aw: " + aw);
                chatManager.addLine("Ability: " + ability);
                chatManager.addLine("Dice: " + diceroll + " rp: " + rp + " fw: " + fw + " aw: " + aw);

                if (rp >= minRP)
                {
                    chatManager.addLine("Success!");
                    ability.applySuccess(version, rp, targets);
                }
                else
                {
                    chatManager.addLine("Failed!");
                    ability.applyFailure(version, rp, user, targets);
                }
            }
            else
            {
                Debug.Log("Cannot use ability " + ability.ToString() + " yet.");
            }
        }

        /**
         * TODO: might be needed for prolonged tests. However, I might just spawn instances for these tests instead?
         */
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
