using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class InteractionManager : MonoBehaviour
    {
        Button[] OptionButtons;

        void Awake()
        {
            Constants.InteractionUI = gameObject;

            // get option buttons
            List<Button> optionButtons = new List<Button>(10);
            foreach (var c in GetComponentsInChildren<Button>())
                if (c.name.StartsWith("Option"))
                    optionButtons.Add(c);
            OptionButtons = optionButtons.ToArray();
        }

        public void StartInteraction()
        {
            Constants.StatusUI.SetActive(false); // hide standard ui
            // TODO: Define interaction options
            // TODO: Define add first options to ui
        }

        public void EndInteraction()
        {
            Constants.StatusUI.SetActive(true);
        }

        void SetCorrectFontSize(int screenWidth, int screenHeight)
        {

        }
    }
}
