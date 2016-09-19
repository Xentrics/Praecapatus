using Assets.Scripts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    /**
     * ADD THIS TO THE SHORT-KEY-BAR COMPONENT
     * - this class merely handles visuals, not any game/input logic
     */
    class ShortKeyBarManager : MonoBehaviour
    {
        // Buttons
        Button[] buttons;
        Color butPressColor = Color.red;
        Color butDefaultColor;
        bool[] pressed;
        bool bAnyButtonPressed = false;
        int numButtons = 10;

        void Awake()
        {
            buttons = GetComponentsInChildren<Button>();
            if (buttons.Length != numButtons)
                throw new ArgumentException("ShortKeyBarManager: Number of buttons found does not match the expected number!");

            if (buttons != null && buttons.Length > 0)
                butDefaultColor = buttons[0].image.color;

            pressed = new bool[numButtons];
        }

        public void pressButton(int id)
        {
            buttons[id].image.color = butPressColor;
            pressed[id] = true;
            bAnyButtonPressed = true;
        }

        public void releaseButton(int id)
        {
            buttons[id].image.color = butDefaultColor;
            pressed[id] = false;

            // check if any button is still presse down
            bAnyButtonPressed = false;
            for (int i = 0; i < pressed.Length; ++i)
                bAnyButtonPressed |= pressed[i];
        }

        public bool isAnyButtonPressed()
        {
            return bAnyButtonPressed; // we only need to calc that once a button is released. A bit more code, but less overhead for the input managers and all...
        }
    }
}
