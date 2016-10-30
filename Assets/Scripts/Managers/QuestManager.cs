using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Assets.Scripts.Quests;
using System;

namespace Assets.Scripts.Managers
{
    public class QuestManager : MonoBehaviour
    {
        /**
         * ATTRIBUTE 
         *************/
        public static string resourcePath = "/Resources/Quests/";
        public static string saveDataPath = "QuestSaveData.xml";

        [SerializeField] List<Quest> acceptedQuests;
        [SerializeField] HashSet<string> finishedQuests;

        /**
         * STATIC FUNCTIONS 
         ********************/
        static Dictionary<string, string> XmlNodesToDic(XmlNodeList nodes, string attr)
        {
            Dictionary<string, string> nodeDic = new Dictionary<string, string>(nodes.Count + 1);
            foreach (XmlNode node in nodes)
                if (!node.InnerText.Equals(""))
                    nodeDic[node.Attributes[attr].Value] = node.InnerText;

            return nodeDic;
        }

        static List<IQuestGoal> ParseGoals(string descs, string types, string datas)
        {
            List<IQuestGoal> parsedGoals = new List<IQuestGoal>();
            //throw new NotImplementedException();
            return parsedGoals;
        }

        public static Quest[] ReadQuestFromGraphml(string file)
        {
            /* load and prepare data */
            XmlDocument xmlDoc = new XmlDocument();
            FileStream stream = new FileStream(resourcePath + file + ".graphml", FileMode.Open);
            xmlDoc.Load(stream);
            Quest[] quests = ReadQuestFromGraphml(xmlDoc, file);
            stream.Close();
            return quests;
        }

        public static Quest[] ReadQuestFromGraphml(TextAsset file)
        {
            /* load and prepare data */
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(file.text);
            return ReadQuestFromGraphml(xmlDoc, file.name);
        }

        public static Quest[] ReadQuestFromGraphml(XmlDocument doc, string filename)
        {
            if (doc == null)
                throw new NullReferenceException("Cannot load graph from NULL reference!");

            XmlNodeList conNodeList = doc.GetElementsByTagName("node");
            XmlNodeList conEdgeList = doc.GetElementsByTagName("edge");
            Dictionary<string, QuestNode> questNodes = new Dictionary<string, QuestNode>();
            List<QuestNode> conStartNodes = new List<QuestNode>(); // store conversation starts

            /* load nodes aka dialogue */
            QuestNode latestNode;
            foreach (XmlNode node in conNodeList)
            {
                latestNode = new QuestNode();
                latestNode.xmlID = node.Attributes["id"].Value;
                Debug.Assert(!questNodes.ContainsKey(latestNode.xmlID), "Duplicated nodes entries found in XML!");
                questNodes[latestNode.xmlID] = latestNode;

                // get important ATTRIBUTES for this node
                Dictionary<string, string> nodeDic = XmlNodesToDic(node.ChildNodes, "key");
                nodeDic.TryGetValue("label", out latestNode.label);

                if (nodeDic.ContainsKey("alignment"))
                    int.TryParse(nodeDic["alignment"], out latestNode.alignment);
                if (nodeDic.ContainsKey("isstart"))
                    conStartNodes.Add(latestNode);

                if (nodeDic.ContainsKey("activationevent"))
                {
                    Debug.Log(nodeDic["activationevent"] + ": Node Activation Event found!");
                }

                if (nodeDic.ContainsKey("requirement"))
                {
                    Debug.Log(nodeDic["requirement"] + ": Node requirement found!");
                }

                if (nodeDic.ContainsKey("goaldescs") && nodeDic.ContainsKey("goaltypes") && nodeDic.ContainsKey("goaldatas"))
                    latestNode.AddGoals(ParseGoals(nodeDic["goaldescs"], nodeDic["goaltypes"], nodeDic["goaldatas"]));
            }

            /* load edges aka responses */
            foreach (XmlNode edge in conEdgeList)
            {
                QuestOption option = new QuestOption();
                Dictionary<string, string> edgeDic = XmlNodesToDic(edge.ChildNodes, "key");
                option.nextNode = questNodes[edge.Attributes["target"].Value];   // set target node link
                questNodes[edge.Attributes["source"].Value].AddOption(option, edgeDic.ContainsKey("isside") && edgeDic["isside"].Equals("true"));   // set source node link                   

                // get important ATTRIBUTES for this edge
                edgeDic.TryGetValue("edgelabel", out option.label);

                if (edgeDic.ContainsKey("alignment"))
                    int.TryParse(edgeDic["alignment"], out option.alignment);

                if (edgeDic.ContainsKey("requirement"))
                {
                    Debug.Log(edgeDic["requirement"] + ": Node requirement found!");
                }

                if (edgeDic.ContainsKey("goaldescs") && edgeDic.ContainsKey("goaltypes") && edgeDic.ContainsKey("goaldatas"))
                    option.AddGoals(ParseGoals(edgeDic["goaldescs"], edgeDic["goaltypes"], edgeDic["goaldatas"]));
            }


            // separate nodes into multiple conversations, if necessary
            Quest[] newConArr = new Quest[(conStartNodes.Count > 0) ? conStartNodes.Count : 1];
            QuestNode[] questNodesArr = new QuestNode[questNodes.Count];
            questNodes.Values.CopyTo(questNodesArr, 0);

            if (conStartNodes.Count == 0)
            {
                Debug.LogError("A valid conversion file should ALWAYS have at least 1 conversation start!");
            }
            else
            {
                for (int i = 0; i < conStartNodes.Count; ++i)
                    newConArr[i] = new Quest(conStartNodes[i], questNodesArr, filename);
            }

            return newConArr;
        }

        /**
         * MEMBER FUNCTIONS 
         ********************/

        void Awake()
        {
            resourcePath = Application.dataPath + resourcePath;
            saveDataPath = resourcePath + saveDataPath;
            LoadFromDisk();
        }

        void OnApplicationQuit()
        {
            SaveToDisk();
        }

        public void LoadFromDisk()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(QuestSaveData));
                FileStream stream = new FileStream(saveDataPath, FileMode.Open);
                QuestSaveData questSave = serializer.Deserialize(stream) as QuestSaveData;
                stream.Close();

                acceptedQuests = new List<Quest>(questSave.acceptedQuests.Length + 5);
                finishedQuests = new HashSet<string>(questSave.finishedQuests);

                foreach (QuestListEntry q in questSave.acceptedQuests)
                {
                    Quest qu = ReadQuestFromGraphml(q.questname)[0];
                    qu.SetToNode(q.nodeID);
                    acceptedQuests.Add(qu);
                }

                Debug.Log("Quests loaded from disk.");
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                acceptedQuests = new List<Quest>();
                finishedQuests = new HashSet<string>();
                return;
            }
        }

        public void SaveToDisk()
        {
            if (!Constants.gameLogic.shouldSaveData)
                return;

            QuestSaveData saveData = new QuestSaveData();
            saveData.acceptedQuests = new QuestListEntry[acceptedQuests.Count];
            for (int i=0; i<acceptedQuests.Count; ++i)
            {
                saveData.acceptedQuests[i].questname = acceptedQuests[i].title;
                saveData.acceptedQuests[i].nodeID = acceptedQuests[i].currentNodeId;
            }
            saveData.finishedQuests = new string[finishedQuests.Count];
            finishedQuests.CopyTo(saveData.finishedQuests);

            XmlSerializer serializer = new XmlSerializer(typeof(QuestSaveData));
            FileStream stream = new FileStream(saveDataPath, FileMode.Create);
            serializer.Serialize(stream, saveData);
            stream.Close();

            Debug.Log("Quests saved to disk.");
        }
    }


    [Serializable]
    public struct QuestListEntry
    {
        [XmlAttribute("name")] public string questname;
        [XmlAttribute("id")]   public string nodeID;
    }


    [Serializable, XmlRoot]
    public class QuestSaveData
    {
        [XmlArray]
        [XmlArrayItem("Q")]
        public QuestListEntry[] acceptedQuests;
        [XmlArray]
        [XmlArrayItem("FQ")]
        public string[] finishedQuests;
    }
}
