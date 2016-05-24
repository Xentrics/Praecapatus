using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using UnityEngine.UI;
using System;
using Assets.Scripts.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Exception;
using Assets.Scripts.Entity;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(InputField))]
    class ChatManager : MonoBehaviour
    {
        bool allowChat = true;
        bool isChatting = false;
        CommandParser cmdParser;
        PlayerController playerC;
        Text contentText;
        public InputField chatInputField;

        protected string chatline = "";
        /*
         * chat line is build by concatenating entries in 'lines'
         * that is chatline = concat(lines[from .. to]) defined by the additional two variables below
         * -1 means no entry present
         */
        protected List<string> lines = new List<string>();
        protected int fromLineIndex = -1;
        protected int toLineIndex = -1;
        protected int maxLinesToShow = 7; // the maximum number of line before we use a sub-array of lines

        protected int maxCharNumberPerLine = 55; // how many characters can a line have before splitting it up

        void Awake()
        {
            if (chatInputField == null)
                chatInputField = GetComponent<InputField>();
            chatInputField.placeholder.GetComponent<Text>().text = "<press tab to type command>";
            foreach (Text o in GetComponentsInChildren<Text>())
            {
                if (o.name == "ContentText")
                    contentText = o;
            }

            if (contentText == null)
                throw new NullReferenceException("Did not find contentText component!");
        }

        void Start()
        {
            playerC = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<PlayerController>();
            cmdParser = new CommandParser(playerC);
            contentText.text = chatline;

            if (maxLinesToShow > 0)
            {
                fromLineIndex = 0;
                toLineIndex = maxLinesToShow-1;

                for (int i = 0; i < maxLinesToShow; i++)
                    lines.Add("");
            }
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

        public void addLine(string l)
        {
            if (String.IsNullOrEmpty(l))
                return;

            if (l.Length > maxCharNumberPerLine)
            {
                // input line is too long. Divide into sub strings
                for (int c = 0; c + maxCharNumberPerLine < l.Length; c += maxCharNumberPerLine)
                {
                    addLine(l.Substring(c, maxCharNumberPerLine));
                }

                int ind = (l.Length / maxCharNumberPerLine) * maxCharNumberPerLine;
                int num = (l.Length % maxCharNumberPerLine);
                l = l.Substring(ind, num); // override l to add the last line part
            }

            // no lines were added till now
            lines.Add(l);
            if (lines.Count <= 1)
            {
                fromLineIndex = 0;
                toLineIndex = 0;
            }
            else
            {
                toLineIndex += 1;
            }

            if (toLineIndex - fromLineIndex + 1 > maxLinesToShow)
                fromLineIndex = toLineIndex - maxLinesToShow + 1; // do not show the first line anymore
            chatline = String.Join("\n", lines.GetRange(fromLineIndex, toLineIndex - fromLineIndex + 1).ToArray());
            contentText.text = chatline;
        }
    }
}