using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using Assets.Scripts.Items;
using Assets.Scripts.Entity;

namespace Assets.Scripts.Managers
{
    [System.Serializable]
    public class XmlDBManager : MonoBehaviour
    {
        /**
         * TODO: encryption code can be found in 'useful'
         ***/

        public static Sprite NotSetIcon = null;
        public const string itemDBPath = "/Resources/XML/itemdb.xml";
        public ItemDB itemDB;

        public static string mainPlayerPath = "/Resources/XML/mainPlayer.xml";

        void Awake()
        {
            
        }

        void Start()
        {
            Debug.Log("Loading data ...");

            /* item db related */
            if (!NotSetIcon)
                NotSetIcon = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/Icons/NotSetIcon", typeof(Sprite));
            if (NotSetIcon)
                throw new System.NullReferenceException("NotSetIcon asset not found!");

            LoadItemDB();

            /* main character */
            LoadEntityData(Constants.gameLogic.pc, mainPlayerPath);

            Debug.Log("Finished loading.");
        }

        void OnApplicationQuit()
        {
            Debug.Log("Saving data ...");

            /* item db related */
            SaveItemDB();
            /* main character */
            SaveEntityData(Constants.gameLogic.pc, mainPlayerPath);

            Debug.Log("Finished saving.");
        }


        // save character data
        public void SaveEntityData(EntityController ec, string path)
        {
            EntitySaveData entData = new EntitySaveData();
            entData.abiList = ec.abiList;
            entData.attrGrpList = ec.attrGrpList;
            entData.attrOtherList = ec.attrOtherList;
            // TODO: fill entData here

            XmlSerializer serializer = new XmlSerializer(typeof(EntitySaveData));
            FileStream stream = new FileStream(Application.dataPath + path, FileMode.Create);
            serializer.Serialize(stream, entData);
            stream.Close();

            if (Object.ReferenceEquals(ec, Constants.gameLogic.pc))
                Debug.Log("Main Character loaded.");
            else
                Debug.Log("Entity loaded.");
        }

        // load character data
        public void LoadEntityData(EntityController ec, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EntitySaveData));
            FileStream stream = new FileStream(Application.dataPath + path, FileMode.Open);
            EntitySaveData entSave = serializer.Deserialize(stream) as EntitySaveData;
            stream.Close();

            if (entSave == null)
                throw new FileNotFoundException("entSave was not loaded. SaveFile not present at: " + path);

            ec.abiList = entSave.abiList;
            ec.attrGrpList = entSave.attrGrpList;
            ec.attrOtherList = entSave.attrOtherList;
            // TODO: read entData here

            if (Object.ReferenceEquals(ec, Constants.gameLogic.pc))
                Debug.Log("Main Character loaded.");
            else
                Debug.Log("Entity loaded.");
        }

        // save items
        public void SaveItemDB()
        {
            // open a new xml file
            XmlSerializer serializer = new XmlSerializer(typeof(ItemDB));
            FileStream stream = new FileStream(Application.dataPath + itemDBPath, FileMode.Create);
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

        // load items
        public void LoadItemDB()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ItemDB));
            FileStream stream = new FileStream(Application.dataPath + itemDBPath, FileMode.Open);
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
            /* item check */
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


    [System.Serializable]
    public class AbilitySaveData
    {
        [XmlAttribute("abi")]  public Abilities.EAbilities abi;
        [XmlAttribute("fw")]   public int fw;
        [XmlAttribute("mode")] public Abilities.EUsageMode mode;
    }


    [System.Serializable]
    public class AttributeGroupSaveData
    {
        [XmlAttribute("attr")] public EAttributeGroup attr;
        [XmlAttribute("val")]  public int val;
    }


    [System.Serializable]
    public class AttributeOtherSaveData
    {
        [XmlAttribute("attr")] public EAttributeOther attr;
        [XmlAttribute("val")]  public int val;
    }


    [System.Serializable]
    [XmlRoot("EntityData")]
    public class EntitySaveData
    {
        [XmlArray("Abilities")]
        [XmlArrayItem("Abi")]
        public List<AbilitySaveData> abiList;
        [XmlArray("AttrGrp")]
        [XmlArrayItem("Attr")]
        public List<AttributeGroupSaveData> attrGrpList;
        [XmlArray("AttrOther")]
        [XmlArrayItem("Attr")]
        public List<AttributeOtherSaveData> attrOtherList;
    }
}
