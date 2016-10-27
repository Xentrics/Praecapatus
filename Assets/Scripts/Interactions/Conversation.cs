using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Assets.Scripts.Interactions
{
    /**
     * any kind of event that can be triggered during events
     */
    public enum EConOptionType
    {
        UNSET,
        NORMAL,
        EXIT,
        OPEN_SHOP
    }

    public class Conversation
    {
        // Conversation related stuff
        public static readonly int MAX_OPTIONS = 5;


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

            public string[] GetResponseStrings()
            {
                Debug.Assert(_responses.Count > 0, "No responses set up for conversation node?!");
                string[] responses = new string[_responses.Count];
                for (int i = 0; i < _responses.Count; ++i)
                    responses[i] = _responses[i].responseText;
                return responses;
            }

            public void AddResponse(BasicDialResponse resp)
            {
                if (_responses.Count + 1 > MAX_OPTIONS)
                    throw new System.Exception("To many responses to add!");
                _responses.Add(resp);
            }

            public BasicDialResponse GetResponse(int id)
            {
                if (id < 0 || id >= _responses.Count)
                    throw new ArgumentOutOfRangeException("Cannot choose response with invalid id!" + id);
                else
                    return _responses[id];
            }

            public bool HasOptions()
            {
                return (_responses != null && _responses.Count > 0);
            }

            public int numOptions()
            {
                return _responses.Count;
            }

            public string XmlID
            {
                get { return _XmlID;  }
                set { _XmlID = value; }
            }

            public string dialogueText
            {
                get { return _dialogueText; }
                set
                {
                    if (value == null)
                        throw new NullReferenceException("Question text must not be null!");
                    _dialogueText = value;
                }
            }

            public override string ToString()
            {
                string str = "";
                foreach (BasicDialResponse r in _responses)
                    str = str + "[" + r.responseText + "] ";
                return str;
            }
        }

        class BasicDialResponse
        {
            protected string _responseText = "";
            public BasicDialNode nextNode = null;
            public EConOptionType optionType = EConOptionType.UNSET;

            public string responseText
            {
                get { return _responseText; }

                set
                {
                    if (value == null)
                        throw new NullReferenceException("ResponseTexts cannot be null!");
                    _responseText = value;
                }
            }

            public bool isExitResponse()
            {
                return nextNode == null;
            }
        }

        string label = "UNNAMED";

        BasicDialNode[] conNodes;
        BasicDialNode startNode;
        BasicDialNode _currentNode;
        List<BasicDialResponse> selections = new List<BasicDialResponse>();


        /**
         * func: changes the currentNode state to the next node based on the chosen response with id 'id'
         * @id: a valid index for the available responses of the current node
         * @return: special signals like conversation exits
         */
        public EConOptionType chose(int id)
        {
            BasicDialResponse resp = _currentNode.GetResponse(id);
            switch(resp.optionType)
            {
                case EConOptionType.UNSET:
                case EConOptionType.NORMAL:
                    if (resp.nextNode.HasOptions())
                    {
                        _currentNode = resp.nextNode;
                        return EConOptionType.NORMAL;
                    }
                    else
                    {
                        _currentNode = resp.nextNode;
                        return EConOptionType.EXIT;
                    }
                default:
                    return resp.optionType;
            }
            
        }

        public string getDialogue()
        {
            return _currentNode.dialogueText;
        }

        /**
         * func: return all options for the current layer in order
         */
        public string[] getOptionStrings()
        {
            if (!_currentNode.HasOptions())
                return null;

            return _currentNode.GetResponseStrings();
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

        static readonly string START_NODE_COLOR = "#00CCFF";    // each conversation needs a node with a certain color code

        static readonly string OPENSHOP_EDGE_COLOR = "#993366"; // a open shop node may never be a start node


        /**
         * func: loads a conversion saved as a graphml file
         * the expected graphml syntax is as follows:
         * 
         * - allows 1 dimensional grouping inside the graph. Stacked grouping may cause unexpected behaviour
         * - a group is considered a separate conversation, but edge can connect them
         * @graphmlFile: Asset reference to a valid graphml file
         */
        public static Conversation[] loadFromYedGraphml(TextAsset graphmlFile)
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
                                        lastNode.dialogueText = n.InnerText;
                                        // TODE: set up multi dialogue texts HERE!
                                    }
                                    else if (n.Name.Equals("y:Fill"))
                                    {
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
            foreach (XmlNode edge in conEdgeList)
            {
                BasicDialResponse response = new BasicDialResponse();
                response.nextNode = dialNodes[edge.Attributes["target"].Value];     // set target node link
                dialNodes[edge.Attributes["source"].Value].AddResponse(response);   // set source node link

                foreach (XmlNode i in edge.ChildNodes)
                {
                    // get the label -> response string
                    if (i.Attributes["key"].Value.Equals(EDGE_GRAPHICS_KEY))
                    {
                        XmlNode PolyLineEdge = i.ChildNodes[0];
                        Debug.Assert(PolyLineEdge.Name.Equals("y:PolyLineEdge"), "Unexpected edge structure!");
                        bool labelFound = false;
                        foreach (XmlNode n in PolyLineEdge.ChildNodes)
                            if (n.Name.Equals("y:LineStyle"))
                            {
                                if (n.Attributes["color"].Value.Equals(OPENSHOP_EDGE_COLOR))
                                    response.optionType = EConOptionType.OPEN_SHOP;
                            }
                            else if (n.Name.Equals("y:EdgeLabel"))
                            {
                                labelFound = true;
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


        public static XGMLGraph loadFromXGML(TextAsset f)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XGMLGraph));
            StringReader r = new StringReader(f.text);
            XGMLGraph xmlDoc = serializer.Deserialize(r) as XGMLGraph;
            r.Close();

            // TODO: extract information for conversation

            return xmlDoc;
        }

        public static Dictionary<string, string> XmlNodesToDic(XmlNodeList nodes, string attr)
        {
            Dictionary<string, string> nodeDic = new Dictionary<string, string>(nodes.Count + 1);
            foreach (XmlNode node in nodes)
                if (!node.InnerText.Equals(""))
                    nodeDic[node.Attributes[attr].Value] = node.InnerText;

            return nodeDic;
        }

        public static Conversation[] loadFromGephiGraphML(TextAsset f)
        {
            if (f == null)
                throw new NullReferenceException("Cannot load graph from NULL reference!");

            /* load and prepare data */
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(f.text);

            XmlNodeList conKeyList = xmlDoc.GetElementsByTagName("key");
            XmlNodeList conNodeList = xmlDoc.GetElementsByTagName("node");
            XmlNodeList conEdgeList = xmlDoc.GetElementsByTagName("edge");
            Dictionary<string, BasicDialNode> dialNodes = new Dictionary<string, BasicDialNode>();
            List<BasicDialNode> conStartNodes = new List<BasicDialNode>(); // store conversation starts

            /* get data keys */
            string PlayerEventNodeId = null;
            string ActivationEventNodeId = null;
            string PlayerEventEdgeId = null;
            string ActivationEventEdgeId = null;
            string RequirementEdgeId = null;

            foreach (XmlNode key in conKeyList)
            {
                switch(key.Attributes["attr.name"].Value)
                {
                    case "PlayerEvent":
                        if (key.Attributes["for"].Value.Equals("node"))
                            PlayerEventNodeId = key.Attributes["id"].Value;
                        else
                            PlayerEventEdgeId = key.Attributes["id"].Value;
                        break;
                    case "ActivationEvent":
                        if (key.Attributes["for"].Value.Equals("node"))
                            ActivationEventNodeId = key.Attributes["id"].Value;
                        else
                            ActivationEventEdgeId = key.Attributes["id"].Value;
                        break;
                    case "Requirement":
                        RequirementEdgeId = key.Attributes["id"].Value;
                        break;
                }
            }

            /* load nodes aka dialogue */
            BasicDialNode latestNode;
            foreach (XmlNode node in conNodeList)
            {
                latestNode = new BasicDialNode();
                latestNode.XmlID = node.Attributes["id"].Value;
                Debug.Assert(!dialNodes.ContainsKey(latestNode.XmlID), "Duplicated nodes entries found in XML!");
                dialNodes[latestNode.XmlID] = latestNode;

                // get important ATTRIBUTES for this node
                Dictionary<string, string> nodeDic = XmlNodesToDic(node.ChildNodes, "key");
                Debug.Assert(nodeDic["label"] != null, "A node should contain a label!");
                latestNode.dialogueText = nodeDic["label"];
                if (nodeDic.ContainsKey("notetype"))
                {
                    string nodetype = nodeDic["notetype"];
                    switch (nodetype)
                    {
                        case "Start":
                            conStartNodes.Add(latestNode);
                            break;
                        case "OpenUI": // doesn't do much here
                            break;
                        case "Exit":   // doesn't do much here
                            break;
                        default:
                            Debug.LogError("Unknown Node Type detected: " + nodetype + " for node: " + latestNode.XmlID);
                            break;
                    }
                }

                if (nodeDic.ContainsKey(PlayerEventNodeId))
                {
                    Debug.Log(nodeDic[PlayerEventNodeId] + ": Node Activation Event found!");
                }

                if (nodeDic.ContainsKey(ActivationEventNodeId))
                {
                    Debug.Log(nodeDic[ActivationEventNodeId] + ": Node Player Event found!");
                }
            }

            /* load edges aka responses */
            foreach (XmlNode edge in conEdgeList)
            {
                BasicDialResponse response = new BasicDialResponse();
                response.nextNode = dialNodes[edge.Attributes["target"].Value];     // set target node link
                dialNodes[edge.Attributes["source"].Value].AddResponse(response);   // set source node link

                // get important ATTRIBUTES for this edge
                Dictionary<string, string> edgeDic = XmlNodesToDic(edge.ChildNodes, "key");
                Debug.Assert(edgeDic["edgelabel"] != null, "An edge should contain a label!");
                response.responseText = edgeDic["edgelabel"];

                if (edgeDic.ContainsKey("edgetype"))
                {
                    switch (edgeDic["edgetype"])
                    {
                        case "OpenShop":
                            response.optionType = EConOptionType.OPEN_SHOP;
                            break;
                        case "Exit":
                            response.optionType = EConOptionType.EXIT;
                            break;
                        default:
                            Debug.LogError("Unknown edge type: " + edgeDic["edgetype"]);
                            break;
                    }
                }
                else
                    response.optionType = EConOptionType.NORMAL;

                if (edgeDic.ContainsKey(PlayerEventEdgeId))
                {
                    Debug.Log(edgeDic[PlayerEventEdgeId] + ": Requirement found!");
                }

                if (edgeDic.ContainsKey(ActivationEventEdgeId))
                {
                    Debug.Log(edgeDic[ActivationEventEdgeId] + ": Requirement found!");
                }

                if (edgeDic.ContainsKey(RequirementEdgeId))
                {
                    Debug.Log(edgeDic[RequirementEdgeId] + ": Requirement found!");
                }
            }


            // separate nodes into multiple conversations, if necessary
            Conversation[] newConArr = new Conversation[(conStartNodes.Count > 0) ? conStartNodes.Count : 1];
            BasicDialNode[] dialNodesArr = new BasicDialNode[dialNodes.Count];
            dialNodes.Values.CopyTo(dialNodesArr, 0);

            if (conStartNodes.Count == 0)
            {
                Debug.LogError("A valid conversion file should ALWAYS have at least 1 conversation start!");
            }
            else
            {
                for (int i = 0; i < conStartNodes.Count; ++i)
                {
                    Conversation newCon = new Conversation();
                    newCon.conNodes = dialNodesArr;
                    newCon.startNode = conStartNodes[i];
                    newCon._currentNode = conStartNodes[i];
                    newConArr[i] = newCon;
                    Debug.Log(newCon.startNode);
                }
            }

            return newConArr;
        }
    }


    [Serializable, XmlRoot(ElementName ="graphml", Namespace = "http://graphml.graphdrawing.org/xmlns")]
    public class GraphMLGraph
    {
        [XmlElement("key")] public List<Key> keys;
        [XmlElement]        public Graph     graph;

        [Serializable]
        public class Key
        {
            [XmlAttribute("attr.name")] public string name;
            [XmlAttribute("attr.type")] public string type;
            [XmlAttribute("for")]       public string _for;
            [XmlAttribute]              public string id;
        }

        [Serializable]
        public class Graph
        {
            [XmlElement("node")] public List<Node> nodes;
            [XmlElement("edge")] public List<Edge> edges;
        }

        [Serializable]
        public class Node
        {
            [XmlAttribute]       public string     id;
            [XmlElement("data")] public List<Data> datas;
            private Dictionary<string, string> _dataDic;

            public Dictionary<string, string> dataDic
            {
                get
                {
                    if (_dataDic != null)
                        return _dataDic;
                    else
                    {
                        Dictionary<string, string> dDic = new Dictionary<string, string>(datas.Count + 1);
                        foreach (Data d in datas)
                            dDic[d.key] = d.value;

                        _dataDic = dDic;
                        return dDic;
                    }
                }
            }
        }

        [Serializable]
        public class Edge
        {
            [XmlAttribute]         public string     source;
            [XmlAttribute]         public string     target;
            [XmlElement("data")]   public List<Data> datas;
        }

        [Serializable]
        public class Data
        {
            [XmlAttribute]
            public string key;
            [XmlText]
            public string value;
        }
    }


    [Serializable, XmlRoot("section")]
    public class XGMLGraph
    {
        [XmlAttribute] public string name;

        [XmlElement("attribute")] public List<XGMLAttribute> attributes;
        public XGMLGraphSection section;


        [Serializable]
        public class XGMLAttribute
        {
            [XmlText] public string attribute;
            [XmlAttribute] public string key;
            [XmlAttribute] public string type;
        }


        [Serializable]
        public class XGMLGraphSection
        {
            [XmlAttribute]
            public string name;

            [XmlElement("attribute")] public List<XGMLAttribute>    attributes;
            [XmlElement("section")]   public List<XGMLNodeEdgeSection>  sections;
        }


        [Serializable]
        public class XGMLNodeEdgeSection
        {
            [XmlAttribute] public string name;

            [XmlElement("attribute")] public List<XGMLAttribute>    attributes;
            [XmlElement("section")]   public List<XGMLDataSection>  sections;

            [Serializable]
            public class XGMLDataSection
            {
                [XmlAttribute] public string name;

                [XmlElement("attribute")] public List<XGMLAttribute> attributes;
            }
        }
    }
}
