using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace Assets.Scripts.Conversations
{
    public class Conversation
    {
        // Conversation related stuff
        public static readonly int MAX_OPTIONS = 5;

        /**
         * any kind of event that can be triggered during events
         */
        enum ETriggerTypes
        {
            Exit
        }

        /**
         * container for trigger instances
         */
        struct OptionTrigger
        {
            public ETriggerTypes type;
            public int optionID;
        }


        /**
         * Node for conversation options
         * - contains a question text that is shown to the player
         * - contains responses for this text, which lead to other dialogue nodes
         */
        class BasicDialNode
        {
            /* if one of the lower two is set, they dominate */
            protected string _XmlID;
            protected string _dialogueText = null;
            protected List<BasicDialResponse> _responses = new List<BasicDialResponse>(MAX_OPTIONS);

            public List<OptionTrigger> conTriggers;

            public void chose(int id)
            {
                throw new NotImplementedException();
            }

            public string[] getOptionStrings()
            {
                string[] responses = new string[_responses.Count];
                for (int i = 0; i < _responses.Count; ++i)
                    responses[i] = _responses[i].responseText;
                return responses;
            }

            public bool hasOptions()
            {
                return (_responses != null && _responses.Count > 0);
            }

            public int numOptions()
            {
                return _responses.Count;
            }

            public string XmlID
            {
                get
                {
                    return _XmlID;
                }

                set
                {
                    _XmlID = value;
                }
            }

            public string dialogueText
            {
                get
                {
                    return _dialogueText;
                }

                set
                {
                    if (value == null)
                        throw new NullReferenceException("Question text must not be null!");
                    _dialogueText = value;
                }
            }
        }

        class BasicDialResponse
        {
            protected string _responseText = "";
            protected BasicDialNode _nextNode;

            public string responseText
            {
                get
                {
                    return _responseText;
                }

                set
                {
                    if (value == null)
                        throw new NullReferenceException("ResponseTexts cannot be null!");
                    _responseText = value;
                }
            }

            public BasicDialNode nextNode
            {
                get
                {
                    return _nextNode;
                }

                set
                {
                    _nextNode = value;
                }
            }

            public bool isExitResponse()
            {
                return _nextNode == null;
            }
        }

        string label = "UNNAMED";

        BasicDialNode[] conNodes;
        BasicDialNode startNode;
        BasicDialNode _currentNode;
        List<BasicDialResponse> selections = new List<BasicDialResponse>();


        /**
         * func: return all options for the current layer in order
         */
        public string[] getOptionStrings()
        {
            if (!_currentNode.hasOptions())
                return null;

            return _currentNode.getOptionStrings();
        }


        static string GetGroupNodeLabel(XmlNode node)
        {

            // get important attributes for this node
            foreach (XmlNode i in node.ChildNodes)
            {
                if (i.Attributes["key"] != null && i.Attributes["key"].Value.Equals(NODE_GRAPHICS_KEY))
                {
                    if (i.HasChildNodes && i.ChildNodes[0].HasChildNodes
                    && i.ChildNodes[0].ChildNodes[0].HasChildNodes
                    && i.ChildNodes[0].ChildNodes[0].ChildNodes[0].Name.Equals("y:GroupNode")) // group nodes have a certain depth
                    {
                        // we found a group. In case we do not find a label, return something non-null anyway
                        foreach (XmlNode c in i.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes)
                        {
                            if (c.Name.Equals("y:NodeLabel"))
                            {
                                Debug.Assert(!c.InnerText.Equals(""), "Group label is empty?");
                                return c.Value;
                            }
                        }

                        return "-unnamed-";
                    }
                }
            }

            return null;
        }


        // graphml parse related stuff
        static readonly string NODE_ACTIVATION_EVENT_KEY = "d4";
        static readonly string NODE_PLAYER_EVENT_KEY = "d5";
        static readonly string PLAYER_EVENT_KEY = "d6";
        static readonly string NODE_GRAPHICS_KEY = "d8";
        static readonly string EDGE_REQUIREMENT_KEY = "d10";
        static readonly string EDGE_ACTIVATION_EVENT = "d11";
        static readonly string EDGE_PLAYER_EVENT = "d12";
        static readonly string EDGE_GRAPHICS_KEY = "d15";

        static readonly string START_NODE_COLOR = "#00CCFF"; // each conversation needs a node with a certain color code


        /**
         * TODO: make start node maker
         * TODO: start nodes indicate new conversation, because they (can) share the same nodes and do not use up more memory
         * 
         * func: loads a conversion saved as a graphml file
         * the expected graphml syntax is as follows:
         * 
         * - allows 1 dimensional grouping inside the graph. Stacked grouping may cause unexpected behaviour
         * - a group is considered a separate conversation, but edge can connect them
         * @graphmlFile: Asset reference to a valid graphml file
         */
        public static Conversation[] loadFromGraphml(TextAsset graphmlFile)
        {
            if (!graphmlFile)
                throw new NullReferenceException("no xmlFile given!");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(graphmlFile.text);

            XmlNodeList conNodeList = xmlDoc.GetElementsByTagName("node");
            XmlNodeList conEdgeList = xmlDoc.GetElementsByTagName("edge");
            Dictionary<string, BasicDialNode> dialNodes = new Dictionary<string, BasicDialNode>();
            List<string> groupNodes = new List<string>(); // may have some use later on
            List<string> groupLabel = new List<string>();
            List<BasicDialNode> conStartNodes = new List<BasicDialNode>(); // store conversation starts

             // LOAD NODES ~> ANSWERES/QUESTIONS
            BasicDialNode lastNode;
            foreach (XmlNode node in conNodeList)
            {
                Debug.Log("> " + node.Name + " " + node.Attributes["id"].Value);
                // grouping nodes only indicate
                string grpLabel = GetGroupNodeLabel(node);
                if (grpLabel != null)
                {
                    Debug.Log("Group node found!");
                    groupNodes.Add(node.Attributes["id"].Value);
                    groupLabel.Add(grpLabel);
                    continue; // TODO: we may grab the group label later on. Do this in here!
                }

                // normal dialogue node
                lastNode = new BasicDialNode();
                lastNode.XmlID = node.Attributes["id"].Value;
                Debug.Log(">> ID: " + lastNode.XmlID);
                Debug.Assert(!dialNodes.ContainsKey(lastNode.XmlID), "Duplicated nodes entries found in XML!");
                dialNodes[lastNode.XmlID] = lastNode;

                // get important ATTRIBUTES for this node
                foreach (XmlNode i in node.ChildNodes)
                {
                    if (i.Attributes["key"] != null)
                    {
                        if (i.Attributes["key"].Value.Equals(NODE_GRAPHICS_KEY))
                        {
                            if (i.HasChildNodes && i.ChildNodes[0].Name.Equals("y:ShapeNode"))
                            {
                                bool labelFound = false;
                                foreach (XmlNode n in i.ChildNodes[0].ChildNodes)
                                {
                                    if (n.Name.Equals("y:NodeLabel"))
                                    {
                                        labelFound = true;
                                        Debug.Log(n.InnerText);
                                        lastNode.dialogueText = n.InnerText;
                                        // TODE: set up multi dialogue texts HERE!
                                    }
                                    else if (n.Name.Equals("y:Fill"))
                                    {
                                        Debug.Log(n.Attributes["color"].Value);
                                        if (n.Attributes["color"].Value.Equals(START_NODE_COLOR))
                                            conStartNodes.Add(lastNode); // found a conversation start node
                                    }
                                }
                                Debug.Assert(labelFound, "Found a response edge, but no response message??");
                            }
                        }

                        if (i.Attributes["key"].Value.Equals(NODE_ACTIVATION_EVENT_KEY))
                        {
                            Debug.Log(node.Attributes["id"].Value + ": Node Activation Event found!");
                        }

                        if (i.Attributes["key"].Value.Equals(NODE_PLAYER_EVENT_KEY))
                        {
                            Debug.Log(node.Attributes["id"].Value + ": Node Player Event found!");
                        }
                    }
                }
            }

            // LOAD EDGE ~> RESPONSES
            // TODO: connect stuff here!
            foreach (XmlNode edge in conEdgeList)
            {
                Debug.Log("> " + edge.Name + " " + edge.Attributes["id"].Value);
                BasicDialResponse response = new BasicDialResponse();
                response.nextNode = dialNodes[edge.Attributes["target"].Value];

                foreach (XmlNode i in edge.ChildNodes)
                {
                    // get the label -> response string
                    if (i.Attributes["key"].Value.Equals(EDGE_GRAPHICS_KEY))
                    {
                        XmlNode PolyLineEdge = i.ChildNodes[0];
                        Debug.Assert(PolyLineEdge.Name.Equals("y:PolyLineEdge"), "Unexpected edge structure!");
                        bool labelFound = false;
                        foreach (XmlNode n in PolyLineEdge.ChildNodes)
                            if (n.Name.Equals("y:EdgeLabel"))
                            {
                                labelFound = true;
                                Debug.Log(n.InnerText);
                                response.responseText = n.InnerText;
                                // TODO: set up multi response texts HERE
                            }
                        Debug.Assert(labelFound, "Found a response edge, but no response message??");
                    }

                    if (i.Attributes["key"].Value.Equals(EDGE_REQUIREMENT_KEY))
                    {
                        Debug.Log(edge.Attributes["id"].Value + ": Requirement found!");
                    }

                    if (i.Attributes["key"].Value.Equals(EDGE_ACTIVATION_EVENT))
                    {
                        Debug.Log(edge.Attributes["id"].Value + ": Requirement found!");
                    }

                    if (i.Attributes["key"].Value.Equals(EDGE_PLAYER_EVENT))
                    {
                        Debug.Log(edge.Attributes["id"].Value + ": Requirement found!");
                    }
                }
            }

            // separate nodes into multiple conversations, if necessary
            Conversation[] newConArr = new Conversation[(conStartNodes.Count > 0) ? conStartNodes.Count : 1];
            BasicDialNode[] dialNodesArr = new BasicDialNode[dialNodes.Values.Count];
            dialNodes.Values.CopyTo(dialNodesArr, 0); // TODO: may split it up later on

            if (conStartNodes.Count == 0)
            {
                Debug.LogError("A valid conversion file should ALWAYS have at least 1 conversation start!");
            }
            else
            {
                for (int i=0; i<conStartNodes.Count; ++i)
                {
                    Conversation newCon = new Conversation();
                    newCon.conNodes = dialNodesArr;
                    newCon.startNode = conStartNodes[i];
                    newCon._currentNode = conStartNodes[i];
                    newConArr[i] = newCon;
                }
            }


            return newConArr;
        }
    }
}
