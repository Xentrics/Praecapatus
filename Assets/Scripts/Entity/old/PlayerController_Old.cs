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
    [RequireComponent(typeof(PlayerMovement_Old))]
    [RequireComponent(typeof(AbilityManager))]
    [RequireComponent(typeof(TestManager))]
    class PlayerController_Old : EntityController
    {
        PlayerMovement_Old pMoveComp;
        public ChatManager chatManager;
        bool isChatting = false;

        AbstractAbility[] keybarAbilities;

        protected override void Awake()
        {
            base.Awake();
            pMoveComp = GetComponent<PlayerMovement_Old>();
        }

        protected override void Start()
        {
            base.Start();

            if (chatManager == null) throw new NullReferenceException("Chracter needs input field!");
            keybarAbilities = new AbstractAbility[10];
            for (int i = 0; i < 10; ++i)
                keybarAbilities[i] = abiCon.getAbility(EAbilities.null_);
        }

        /*
         * handle correct key input across multiple scripts & mechanics
         */
        protected override void LateUpdate()
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

        public void executeKeybarAbility(int keyID)
        {
            if (keyID >= 0 && keyID <= 9)
            {
                testManager.testInstant(0, keybarAbilities[keyID], this, null);
            }
            else
                throw new ArgumentOutOfRangeException("keyID must be element of [0,9]");
        }

        public void releaseKeybarAbility(int keyID)
        {
            // will be needed for prolonged tests, etc., maybe or so. How knows now?
        }
    }
}
