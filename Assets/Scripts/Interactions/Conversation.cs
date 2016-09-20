using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interactions
{
    public class Conversation : MonoBehaviour
    {
        public static readonly int MAX_OPTIONS = 5;

        /**
         * any kind of event that can be triggered during events
         */
        public enum ETriggerTypes
        {
            Exit
        }

        /**
         * container for trigger instances
         */
        [Serializable]
        public struct OptionTrigger
        {
            public ETriggerTypes type;
            public int optionID;
        }

        [Serializable]
        public struct StringArray
        {
            public string[] s;
        }

        /**
         * special note that allows multiple strings per option and dialogue
         * one string is chosen during instanciation at random
         */
        [Serializable]
        public class Node
        {
            /* set these for randome selection */
            public string[] _randDialogueTexts = null;
            public StringArray[] _randOptionTexts;
            /* if one of the lower two is set, they dominate */
            public string _selText = null;
            public string[] _selOptionTexts;

            public OptionTrigger[] conTriggers;

            private void select()
            {
                Debug.Assert(_randDialogueTexts != null && _randOptionTexts != null, "ArrayNode without dialogue or answeres!");
                
                int selDia = UnityEngine.Random.Range(0, _randDialogueTexts.Length);
                _selText = _randDialogueTexts[selDia];

                _selOptionTexts = new string[_randOptionTexts.Length]; // we choose exactly 1 per first dimension
                for (int i=0; i< _randOptionTexts.Length; ++i)
                {
                    //int sel = UnityEngine.Random.Range(0, _optionTexts[i].Length);
                    //_selectedOptionTexts[i] = _optionTexts[i][sel];
                }
            }

            public void chose(int id)
            {
                throw new NotImplementedException();
            }

            public string getDialogue()
            {
                if (_selText == null)
                    select();
                return _selText;
            }

            public string[] getOptionsStrings()
            {
                if (_selOptionTexts == null)
                    select();
                return _selOptionTexts;
            }

            public bool hasOptions()
            {
                return (_randOptionTexts != null && _randOptionTexts.Length > 0) || (_selOptionTexts != null && _selOptionTexts.Length > 0);
            }

            public int numOptions()
            {
                return (_selOptionTexts == null) ? _randOptionTexts.Length : _selOptionTexts.Length;
            }
        }

        [SerializeField]
        public Node startNode;
        Node _currentNode;
        List<Node> selections = new List<Node>();


        /**
         * func: return all options for the current layer in order
         */
        public string[] getOptionStrings()
        {
            if (!_currentNode.hasOptions())
                return null;

            return _currentNode.getOptionsStrings();
        }
    }
}
