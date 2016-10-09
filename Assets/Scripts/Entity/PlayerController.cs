using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using System;
using Assets.Scripts.Managers;
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
            public int instance_version; // specific modification of the ability for instant use
        };

        KeybarAbility[] keybarAbilities;

        PlayerMovement pMoveComp;
        bool isChatting = false;

        protected bool bInstantUseDuringStory = true; // true: do not show ability version selection even during story mode

        protected float interactionRange = 10f;

        protected override void Awake()
        {
            base.Awake();
            pMoveComp = GetComponent<PlayerMovement>();
        }

        protected override void Start()
        {
            base.Start();

            if (Constants.chatManager == null) throw new NullReferenceException("Chracter needs input field!");
            keybarAbilities = new KeybarAbility[10];
            // DEBUG: implemented abilities
            keybarAbilities[0].abi = abiCon.getAbility(EAbilities.Astrahlbelebung);
            keybarAbilities[0].instance_version = 0;
            keybarAbilities[1].abi = abiCon.getAbility(EAbilities.test);
            keybarAbilities[1].instance_version = 0;
            // placeholder
            for (int i = 2; i < 10; ++i)
            {
                keybarAbilities[i].abi = abiCon.getAbility(EAbilities.null_);
                keybarAbilities[i].instance_version = 0;
            }
        }

        /*
         * func: handle correct key input across multiple scripts & mechanics
         */
        protected override void LateUpdate()
        {
            // check whether or not the player opens/cloeses chat
            if (Constants.chatManager.isChatAllowed() && CrossPlatformInputManager.GetButtonDown("OpenChat"))
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
                    DEB_testManager.testInstant(keybarAbilities[keyID].instance_version, keybarAbilities[keyID].abi, this);
                }
                else
                {
                    //TODO: ability selection mode
                    throw new NotImplementedException("Ability selection mode not available yet!");
                }
            }
            else
                throw new System.ArgumentOutOfRangeException("keyID must be element of [0,9]");
        }

        /**
         * func: try to interact with object in line of sight
         * func: use default interaction range
         * @RETURN: TRUE, if any continued interaction started
         */
        public bool tryInteractWith()
        {
            return tryInteractWith(interactionRange);
        }

        /**
         * func: try to interact with object in line of sight
         * func: use default interaction range
         * @RETURN: TRUE, if any continued interaction started
         */
        public bool tryInteractWith(float maxRange)
        {
            // iterative raycast
            float restRange = maxRange;
            Vector3 startLoc = transform.position;
            while (restRange > 0)
            {
                RaycastHit hitinfo;
                if (Physics.Raycast(startLoc, moveComp.lookDir, out hitinfo, maxRange))
                {
                    PraeObject hit = hitinfo.transform.gameObject.GetComponent<PraeObject>();
                    if (hit != null)
                    {
                        return tryInteractWith(hit); // found our interaction object
                    }
                    else
                    {
                        // hit something, but not the correct thing. prepare next cast
                        restRange -= Vector3.Distance(transform.position, hitinfo.point);
                        startLoc = hitinfo.point; // start from the last hit location
                    }
                }
                else
                {
                    Debug.DrawRay(startLoc, moveComp.lookDir, Color.blue, 2f);
                    return false; // nothing was hit
                }
            }

            return false;
        }

        /**
         * func: try to interact with given object
         * @I: object to interact with
         * @RETURN: TRUE, if any continued interaction started
         */
        public bool tryInteractWith(PraeObject I)
        {
            Debug.Log("Try interaction with: " + I);
            if (I.TryInteract(praeObject))
            {
                // TODO: do stuff
                Debug.Log("Start of interaction");
                return true;
            }
            else
            {
                print(I.description);
                Debug.Log("No interaction available.");
                return false;
            }
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
    }
}
