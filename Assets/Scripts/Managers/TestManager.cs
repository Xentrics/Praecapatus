using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.Entity;
using Assets.Scripts.Abilities;
using Assets.Scripts.Character;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(PraeObject))]
    [RequireComponent(typeof(CharInfo))]
    class TestManager : MonoBehaviour
    {
        PlayerController playerC;
        CharInfo charInfo;
        ChatManager chatManager;
        GameLogic gameManager;

        public void Awake()
        {
            playerC = GetComponent<PlayerController>();
            charInfo = GetComponent<CharInfo>();
        }

        public void Start()
        {
            chatManager = GameObject.FindGameObjectWithTag("ChatBox").GetComponent<ChatManager>();
            gameManager = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameLogic>();
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
        public void testInstant(int version, AbstractAbility ability, PraeObject user, List<PraeObject> targets)
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
        public void testInstant(int version, int minRP, AbstractAbility ability, PraeObject user, List<PraeObject> targets)
        {
            if (ability.canUse(version, user, targets))
            {
                int c = ability.getTestModifier(version, user, targets);            // Erschwernis/Erleichterung
                int aw = charInfo.getAttributeValue(ability.getAttributeGroup());   // Value of assigned attribute
                int fw = ability.fw;                                                // Fertigkeitswert of ability
                ELuck luck = ELuck.normal;

                int diceroll = UnityEngine.Random.Range(1, 21); // [inclusive, exclusive]
                int rp = fw + aw - c - diceroll;

                if (diceroll == 1)
                {
                    // test for luck, "Glücksgriff"
                    int second_roll = UnityEngine.Random.Range(1, 21);
                    if (second_roll <= aw)
                    {
                        rp += 20 + second_roll - aw; // normal rp + second_roll rp + 20
                        luck = ELuck.luck;
                    }
                }
                else if (diceroll == 20)
                {
                    // test for slipup "Patzer"
                    int reroll = UnityEngine.Random.Range(1, 21);
                    if (reroll > aw + Math.Max(0, rp)) // if aw + fp > 20, we add positive rp to the pool
                        luck = ELuck.slipup;
                }

                print("Ability: " + ability + "| Dice: " + diceroll + " rp: " + rp + " fw: " + fw + " aw: " + aw + " luck: " + luck);
                chatManager.addLine("Ability: " + ability);
                chatManager.addLine("Dice: " + diceroll + " rp: " + rp + " fw: " + fw + " aw: " + aw + " luck: " + luck);

                if (rp >= minRP)
                {
                    chatManager.addLine("Success!");
                    ability.applySuccess(version, rp, luck, user, targets);
                }
                else
                {
                    chatManager.addLine("Failed!");
                    ability.applyFailure(version, rp, luck, user, targets);
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
