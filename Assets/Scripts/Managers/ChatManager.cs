using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using UnityEngine.UI;
using System;
using Assets.Scripts.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Exception;
using Assets.Scripts.Player;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(InputField))]
    class ChatManager : MonoBehaviour
    {
        bool allowChat = true;
        bool isChatting = false;
        CommandParser cmdParser;
        PlayerController playerC;
        public InputField chatInputField;

        void Awake()
        {
            if (chatInputField == null)
                chatInputField = GetComponent<InputField>();
            chatInputField.placeholder.GetComponent<Text>().text = "<press tab to type command>";
        }

        void Start()
        {
            playerC = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<PlayerController>();
            cmdParser = new CommandParser(playerC);
        }

        /*
         * handles commands
         * handles chat (if implemented)
         */
        void LateUpdate()
        {
            if (chatInputField != null)
            {             
                // check whether or not the player opens/cloeses chat
                if (allowChat && CrossPlatformInputManager.GetButtonDown("OpenChat"))
                {
                    if (isChatting)
                    {
                        // deactivate chat
                        chatInputField.text = ""; // clear text
                        isChatting = false;
                    }
                    else
                    {
                        // activate chat & disable player movement
                        isChatting = true;
                    }
                }


                // capture keyboard input
                if (isChatting && Input.anyKeyDown)
                {
                    if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        if (chatInputField.text.Length > 0)
                            chatInputField.text = chatInputField.text.Remove(chatInputField.text.Length - 1);
                    }
                    else if (CrossPlatformInputManager.GetButton("Submit"))
                    {
                        if (chatInputField.text.StartsWith("@"))
                        {
                            try
                            {
                                cmdParser.parseCommandLine(chatInputField.text.Substring(1), playerC);
                            }
                            catch (CommandNotFoundException e)
                            {
                                print(e.Message);
                            }
                        }

                        chatInputField.text = ""; // clear current line. TODO: muss line to history
                    }
                    else
                        chatInputField.text += Input.inputString;
                }
            }
            else
            {
                throw new NullReferenceException("Attached ChatManager did not find an Input Field Component!");
            }
        }

        /*
         * should be called to handle the transition from different view modes of the player instead of setting enableChat=false directly
         */
        public void disableChat()
        {
            if (isChatting)
                throw new NotImplementedException("isChatting needs impl.");

            allowChat = false;
        }

        public void enableChat()
        {
            allowChat = true;
        }

        public bool isChatAllowed()
        {
            return allowChat;
        }
    }
}