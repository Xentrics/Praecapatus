using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using Assets.Scripts.Items;

namespace Assets.Scripts.Managers
{
    [System.Serializable]
    public class ItemDBManager : MonoBehaviour
    {
        /**
         * TODO: encryption code can be found in 'useful'
         ***/

        public static Sprite NotSetIcon = null;
        public const string path = "/Resources/XML/itemdb.xml";
        public ItemDB itemDB;

        void Awake()
        {
            if (!NotSetIcon)
                NotSetIcon = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/Icons/NotSetIcon", typeof(Sprite));
            if (NotSetIcon)
                throw new System.NullReferenceException("NotSetIcon asset not found!");

            LoadDB();
        }

        void OnApplicationQuit()
        {
            SaveDB();
        }

        // save
        public void SaveDB()
        {
            // open a new xml file
            XmlSerializer serializer = new XmlSerializer(typeof(ItemDB));
            FileStream stream = new FileStream(Application.dataPath + path, FileMode.Create);
            serializer.Serialize(stream, itemDB); // 
            stream.Close();
#if UNITY_EDITOR
            if (SanityCheck())
                Debug.LogError("One or more items have invalid properties!");
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("AssetDB refresh");
#endif
            Debug.Log("Item database saved to disk.");
        }

        // load
        public void LoadDB()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ItemDB));
            FileStream stream = new FileStream(Application.dataPath + path, FileMode.Open);
            itemDB = serializer.Deserialize(stream) as ItemDB;
            stream.Close();

            if (itemDB == null)
                throw new System.NullReferenceException("ItemDB not found!");
            if (itemDB.items.Count <= 0)
                Debug.LogError("No entries in item database?");
            
            foreach (PraeItem item in itemDB.items)
            {
                // do stuff
                Debug.Log(item.name);
            }
            Debug.Log("ItemDatabase loaded.");
        }

        /**
         * @return TRUE, if every item set is valid
         */
        public bool SanityCheck()
        {
            bool failed = false;
            foreach (PraeItem i in itemDB.items)
            {
                if (!i.SanityCheck())
                {
                    Debug.LogError("Invalid item properties of item: " + i.name);
                    failed = true;
                }
            }

            return false;
        }
    }

    [System.Serializable]
    [XmlRoot("ItemCollection")]
    public class ItemDB
    {
        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<PraeItem> items = new List<PraeItem>();
    }
}
