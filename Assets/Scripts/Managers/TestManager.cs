using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.Entity;
using Assets.Scripts.Abilities;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(PraeObject))]
    [RequireComponent(typeof(EntityInfo))]
    public class TestManager : MonoBehaviour
    {
        PlayerController playerC;
        EntityInfo charInfo;

        public void Awake()
        {
            playerC = GetComponent<PlayerController>();
            charInfo = GetComponent<EntityInfo>();
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

        /**
         * Func: Basic dice roll test function for the use of abilities in the game
         * Func: this version should be called by entity controller for realtime use
         * Func: uses GameManager to determine minimal required RP
         * @version: changes specifics of 'ability'
         * @ability: the one that will be executed
         * @user: entity using the ability
         */
        public void testInstant(int version, AbstractAbility ability, EntityController user)
        {
            int minRP = 0; //TODO: the GameManager should somehow determine this value
            List<PraeObject> targets = ability.getTargets(version, user);
            testInstant(version, minRP, ability, user, targets);
        }

        /**
         * Func: Basic dice roll test function for the use of abilities in the game
         * Func: this version should be called by entity controller when using interfaces for targets
         * Func: uses GameManager to determine minimal required RP
         * @version: changes specifics of 'ability'
         * @ability: the one that will be executed
         * @user: entity using the ability
         * @targets: potential targets for the ability (if necessary)
         */
        public void testInstant(int version, AbstractAbility ability, EntityController user, List<PraeObject> targets)
        {
            int minRP = 0; //TODO: the GameManager should somehow determine this value
            testInstant(version, minRP, ability, user, targets);
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
        public void testInstant(int version, int minRP, AbstractAbility ability, EntityController user, List<PraeObject> targets)
        {
            if (ability.canUse(version, user, targets))
            {
                int c = ability.getTestModifier(version, user, targets);            // Erschwernis/Erleichterung
                int aw = charInfo.getAttributeValue(ability.attributeGroup);   // Value of assigned attribute
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
                Constants.chatManager.addLine("Ability: " + ability);
                Constants.chatManager.addLine("Dice: " + diceroll + " rp: " + rp + " fw: " + fw + " aw: " + aw + " luck: " + luck);

                if (rp >= minRP)
                {
                    Constants.chatManager.addLine("Success!");
                    ability.applySuccess(version, rp, luck, user, targets);
                }
                else
                {
                    Constants.chatManager.addLine("Failed!");
                    ability.applyFailure(version, rp, luck, user, targets);
                }
            }
            else
            {
                Debug.Log("Cannot use ability " + ability.ToString() + ".");
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
