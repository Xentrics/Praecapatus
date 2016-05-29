using Assets.Scripts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    /**
     * ADD THIS COMPONENT TO THE PLAYER
     */
    [RequireComponent(typeof(PlayerController_Old))]
    class PlayerInputManager : MonoBehaviour
    {
        #pragma warning disable 0169
        public Canvas HUDCanvas;
        ShortKeyBarManager barManager;
        PlayerController_Old pc;

        void Awake()
        {
            barManager = HUDCanvas.GetComponentInChildren<ShortKeyBarManager>();
            if (!barManager)
                throw new NullReferenceException("Could not aquire bar manager from given canvas!");
            pc = GetComponent<PlayerController_Old>();
        }

        void Update()
        {
            // handle key-down events
            if (Input.anyKeyDown)
            {
                // handle the numbers 0 to 9 of the upper keyboard bar
                const int alpha1 = (int)KeyCode.Alpha1; // get the keycode to start from
                for (int i = 0; i < 10; ++i)
                    if (Input.GetKeyDown((KeyCode)alpha1 + i))
                    {
                        barManager.pressButton(i);
                        pc.executeKeybarAbility(i);
                    }

                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    barManager.pressButton(9);
                    pc.executeKeybarAbility(9);
                }

                // handle remaining keys
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
            }
        }
    }
}
