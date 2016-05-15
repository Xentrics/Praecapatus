using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using Assets.Scripts.Managers;
using Assets.Scripts.Entity;
using Assets.Scripts.Abilities;

namespace Assets.Scripts.Entity
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(AbilityManager))]
    [RequireComponent(typeof(TestManager))]
    class PlayerController : EntityController
    {
        PlayerMovement moveComp;
        public EntityMovement mcomp;
        AbilityManager abiCon;
        TestManager testManager;
        public ChatManager chatManager;
        bool isChatting = false;

        void Awake()
        {
            moveComp = GetComponent<PlayerMovement>();
            abiCon = GetComponent<AbilityManager>();
            testManager = GetComponent<TestManager>();
            if (chatManager == null) throw new NullReferenceException("Chracter needs input field!");
        }

        /*
         * handle correct key input across multiple scripts & mechanics
         */
        void LateUpdate()
        {
            // check whether or not the player opens/cloeses chat
            if (chatManager.isChatAllowed() && CrossPlatformInputManager.GetButtonDown("OpenChat"))
            {
                if (isChatting)
                {
                    // deactivate chat
                    moveComp.setCinematicMode(false); // disable input to movement component
                    isChatting = false;
                }
                else
                {
                    // activate chat & disable player movement
                    moveComp.setCinematicMode(true);
                    isChatting = true;
                }
            }
        }

        public Vector3 getPosition()
        {
            return moveComp.getPosition();
        }

        public float getMeleeRange()
        {
            return 10; // 1 meter?
        }

        public void executeAbilityWith(EAbilities A, int minRP = 0)
        {
            testManager.testInstant(minRP, abiCon.getAbility(A), this, null);
        }
    }
}
