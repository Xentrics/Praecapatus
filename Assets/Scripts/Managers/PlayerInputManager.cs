using Assets.Scripts.Entity;
using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Assets.Scripts.Managers
{
    /**
     * ADD THIS COMPONENT TO THE PLAYER
     */
    [RequireComponent(typeof(EntityController))]
    class PlayerInputManager : MonoBehaviour
    {
        ShortKeyBarManager barManager;
        PlayerController pc;

        void Start()
        {
            barManager = Constants.HUDCanvas.GetComponentInChildren<ShortKeyBarManager>();
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
                        barManager.pressButton(i-1);
                        pc.executeKeybarAbility(i-1);
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

                if (CrossPlatformInputManager.GetButton("RotateRightwise"))
                    Constants.gameLogic.rotateWorldView(false);

                if (CrossPlatformInputManager.GetButton("RotateLeftwise"))
                    Constants.gameLogic.rotateWorldView(true);
            }

            // check if we have do handle key-up events
            if (barManager.isAnyButtonPressed())
            {
                // we only handle the key bar if the bar had any key pressed
                // if a key is not pressed >anymore<, then we handle this here!
                for (int i = 1; i < 11; ++i)
                {
                    if (CrossPlatformInputManager.GetButtonUp("Action" + i))
                    {
                        barManager.releaseButton(i - 1);
                        pc.releaseKeybarAbility(i - 1);
                        break;
                    }
                }

                // handle remaining keys
            }
        }
    }
}
