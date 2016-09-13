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
        struct KeybarAbility {
            public AbstractAbility abi;
            public int instant_version; // specific modification of the ability for instant use
        };

        KeybarAbility[] keybarAbilities;

        PlayerMovement pMoveComp;
        public ChatManager chatManager;
        bool isChatting = false;

        protected bool bInstantUseDuringStory = true; // true: do not show ability version selection even during story mode

        protected override void Awake()
        {
            base.Awake();
            pMoveComp = GetComponent<PlayerMovement>();
        }

        protected override void Start()
        {
            base.Start();

            if (chatManager == null) throw new NullReferenceException("Chracter needs input field!");
            keybarAbilities = new KeybarAbility[10];
            for (int i = 0; i < 10; ++i)
            {
                keybarAbilities[i].abi = abiCon.getAbility(EAbilities.null_);
                keybarAbilities[i].instant_version = 0;
            }
        }

        /*
         * func: handle correct key input across multiple scripts & mechanics
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
        

        /**
         * func: use the ability set for a specific keyboard key
         * is called from PlayerInputManager
         * @keyID: input key from the keyboard
         */
        public void executeKeybarAbility(int keyID)
        {
            if (keyID >= 0 && keyID <= 9)
            {
                if (bInstantUseDuringStory || bInfight)
                {
                    testManager.testInstant(keybarAbilities[keyID].instant_version, keybarAbilities[keyID].abi, this, null);
                }
                else
                {
                    //TODO: ability selection mode
                    throw new NotImplementedException("Ability selection mode not available yet!");
                }
            }
            else
                throw new ArgumentOutOfRangeException("keyID must be element of [0,9]");
        }

        /**
         * func: load the abilities preset ingame
         */
        public void loadKeybarAbilitiesFromSave()
        {
            //TODO: implement
            throw new NotImplementedException("load keybar abilities from save");
        }

        public void releaseKeybarAbility(int keyID)
        {
            // will be needed for prolonged tests, etc., maybe or so. How knows now?
        }

        public override Vector3 getPosition()
        {
            return pMoveComp.getPosition();
        }

        public override float getMeleeRange()
        {
            return 10; // TODO: 1 meter?
        }
    }
}
