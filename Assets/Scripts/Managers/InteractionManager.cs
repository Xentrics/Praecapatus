﻿using Assets.Scripts.Interactions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class InteractionManager : MonoBehaviour
    {
        Text dialogueText = null;
        Button[] responseButtons;
        Conversation currentConversation;
        InteractionComponent questioner, responder;

        void Awake()
        {
            Constants.InteractionUI = gameObject;
            Constants.interactionManager = this;

            // get option buttons
            List<Button> optionButtons = new List<Button>(10);
            foreach (var c in GetComponentsInChildren<Button>())
                if (c.name.StartsWith("OptionButton"))
                {
                    optionButtons.Add(c);
                    int id = Int32.Parse(c.name.Substring("OptionButton".Length));
                    c.onClick.AddListener(() => { ResponseButtonOnClick(id-1); });
                    c.interactable = true;
                }
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


        void ResponseButtonOnClick(int id)
        {
            chooseResponse(id);
        }

        /**
         * func: sets up first dialogue message and responses
         */
        public void StartConversation(InteractionComponent questioner, InteractionComponent responder, Conversation con)
        {
            if (con == null || questioner == null || responder == null)
                throw new NullReferenceException("Cannot start interaction: con, questioner or responder reference not set!");
            currentConversation = con;
            this.questioner = questioner;
            this.responder = responder;
            Constants.ActivateUI(EUIMode.INTERACTION_UI);

            updateUI();
        }

        public void chooseResponse(int id)
        {
            EConOptionType signal = currentConversation.chose(id);

            switch (signal)
            {
                case EConOptionType.EXIT:
                    EndInteraction();
                    return;
                case EConOptionType.OPEN_SHOP:
                    Debug.Log("r: " + responder + " q: " + questioner);
                    questioner.shop.Register();
                    Constants.ShopUI.GetComponent<ShopManager>().currendManager = this;
                    Constants.ShopUI.GetComponent<ShopManager>().SetInventories(responder.inventory, questioner.shop);
                    Constants.ActivateUI(EUIMode.SHOP_UI);
                    break;
                case EConOptionType.OFFER_QUEST:

                    break;
                default:
                    break;
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
            {
                responseButtons[i].GetComponentInChildren<Text>().text = "  " + (i + 1) + ". " + respTexts[i] + "  ";
                responseButtons[i].interactable = true;
            }

            // handle unused buttons
            for (int i = respTexts.Length; i < responseButtons.Length; ++i)
            {
                responseButtons[i].GetComponentInChildren<Text>().text = ""; // TODO: make that in a nicer way
                responseButtons[i].interactable = false;
            }
        }

        /**
         * func: disable interactionUI and bring up standard UI
         */
        public void EndInteraction()
        {
            Constants.ActivateUI(EUIMode.STATUS_UI);
        }

        void SetCorrectFontSize(int screenWidth, int screenHeight)
        {
            throw new NotImplementedException();
        }
    }
}
