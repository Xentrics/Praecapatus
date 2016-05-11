﻿using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    class PlayerController : MonoBehaviour
    {
        PlayerMovement moveComp;
        public ChatManager chatManager;
        bool isChatting = false;

        void Awake()
        {
            moveComp = GetComponent<PlayerMovement>();
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

        private void getPressedKey()
        {

        }
    }
}
