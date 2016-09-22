using Assets.Scripts.Conversations;
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
        Text dialogueText = null;
        Button[] responseButtons;
        Conversation currentConversation;

        void Awake()
        {
            Constants.InteractionUI = gameObject;
            Constants.interactionManager = this;

            // get option buttons
            List<Button> optionButtons = new List<Button>(10);
            foreach (var c in GetComponentsInChildren<Button>())
                if (c.name.StartsWith("Option"))
                    optionButtons.Add(c);
            responseButtons = optionButtons.ToArray();
            // get dialogue text
            foreach (var c in GetComponentsInChildren<Text>())
                if (c.name.Equals("DialogueText"))
                    dialogueText = c;
        }

        void Start()
        {
            gameObject.SetActive(false);
        }

        /**
         * func: sets up first dialogue message and responses
         */
        public void StartInteraction(Conversation con)
        {
            if (con == null)
                throw new NullReferenceException("Cannot start interaction: con reference not set!");
            currentConversation = con;
            Constants.StatusUI.SetActive(false); // hide standard ui
            Constants.InteractionUI.SetActive(true);

            updateUI();
        }

        public void chooseResponse(int id)
        {
            EConOptionType signal = currentConversation.chose(id);

            switch (signal)
            {
                case EConOptionType.Normal:
                    break;
                case EConOptionType.Exit:
                    EndInteraction();
                    return;
            }

            updateUI();
        }

        /**
         * func: sets up buttons and label based on the current conversation state
         */
        void updateUI()
        {
            // set up conversation button and labels
            dialogueText.text = currentConversation.getDialogue();

            // handle response buttons
            string[] respTexts = currentConversation.getOptionStrings();
            Debug.Assert(responseButtons.Length >= respTexts.Length, "Too few response buttons or too many options!");
            for (int i = 0; i < respTexts.Length; ++i)
                responseButtons[i].GetComponentInChildren<Text>().text = "  " + (i+1) + ". " + respTexts[i] + "  ";
            // handle unused buttons
            for (int i = respTexts.Length; i < responseButtons.Length; ++i)
                responseButtons[i].GetComponentInChildren<Text>().text = ""; // TODO: make that in a nicer way
        }

        /**
         * func: disable interactionUI and bring up standard UI
         */
        public void EndInteraction()
        {
            Constants.StatusUI.SetActive(true);
            Constants.InteractionUI.SetActive(false);
        }

        void SetCorrectFontSize(int screenWidth, int screenHeight)
        {

        }
    }
}
