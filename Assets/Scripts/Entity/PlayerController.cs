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
        PlayerMovement pMoveComp;
        public ChatManager chatManager;
        bool isChatting = false;

        void Awake()
        {
            pMoveComp = GetComponent<PlayerMovement>();
        }

        void Start()
        {
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
                    pMoveComp.setCinematicMode(false); // disable input to movement component
                    isChatting = false;
                }
                else
                {
                    // activate chat & disable player movement
                    pMoveComp.setCinematicMode(true);
                    isChatting = true;
                }
            }
        }

        public override Vector3 getPosition()
        {
            return pMoveComp.getPosition();
        }

        public override float getMeleeRange()
        {
            return 10; // 1 meter?
        }

        public override void executeAbilityWith(EAbilities A, int minRP = 0)
        {
            testManager.testInstant(minRP, abiCon.getAbility(A), this, null);
        }
    }
}
