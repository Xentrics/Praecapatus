using Assets.Scripts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Assets.Scripts.Managers
{
    /**
     * ADD THIS COMPONENT TO THE PLAYER
     */
    [RequireComponent(typeof(PlayerController))]
    class PlayerInputManager : MonoBehaviour
    {
        #pragma warning disable 0169
        public Canvas HUDCanvas;
        ShortKeyBarManager barManager;
        PlayerController pc;

        void Awake()
        {
            barManager = HUDCanvas.GetComponentInChildren<ShortKeyBarManager>();
            if (!barManager)
                throw new NullReferenceException("Could not aquire bar manager from given canvas!");
            pc = GetComponent<PlayerController>();
        }

        void Update()
        {
            // handle key-down events
            if (Input.anyKeyDown)
            {
                // handle the numbers 0 to 9 of the upper keyboard bar
                bool used = false;                      // prevent simultanious use of abilities
                for (int i = 1; i < 11; ++i)
                {
                    if (CrossPlatformInputManager.GetButton("Action"+i))
                    {
                        barManager.pressButton(i);
                        pc.executeKeybarAbility(i);
                        used = true;
                        break;
                    }
                }

                if(!used && CrossPlatformInputManager.GetButton("interact"))
                {
                    if (!pc.tryInteractWith()) // next target in sight
                        Debug.Log("Found nothing to interact with.");
                }

                // handle remaining keys
                if (CrossPlatformInputManager.GetButton("toggleWalk"))
                    pc.toggleRunning();
  
            }

            // check if we have do handle key-up events
            if (barManager.isAnyButtonPressed())
            {
                // we only handle the key bar if the bar had any key pressed
                // if a key is not pressed >anymore<, then we handle this here!
                const int alpha1 = (int)KeyCode.Alpha1; // get the keycode to start from
                for (int i = 0; i < 10; ++i)
                    if (Input.GetKeyUp((KeyCode)alpha1 + i))
                    {
                        barManager.releaseButton(i);
                        pc.releaseKeybarAbility(i);
                    }
                if (Input.GetKeyUp(KeyCode.Alpha0))
                {
                    barManager.releaseButton(9);
                    pc.releaseKeybarAbility(9);
                }

                // handle remaining keys
            }
        }
    }
}
