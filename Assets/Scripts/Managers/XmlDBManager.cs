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
        public static string generalInformationPrefix = "/Resources/XML/GeneralInformation.xml";
        public static string charXMLPrefix = "/Resources/XML/char_";
        public static string shopXMLPrefix = "/Resources/XML/shop_";

        void Awake()
        {
            Constants.xmlHandler = this;

            /* general information */
            LoadGeneralInformation();
        }

        void Start()
        {
            Debug.Log("Loading data ...");

            /* item db related */
            if (!NotSetIcon)
                NotSetIcon = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/Icons/NotSetIcon", typeof(Sprite));
            if (NotSetIcon)
                throw new System.NullReferenceException("NotSetIcon asset not found!");

            /* item db */
            LoadItemDB();

            /* main character */
            LoadEntityData(Constants.gameLogic.pc, mainPlayerPath);

            Debug.Log("Finished loading.");
        }

        void OnApplicationQuit()
        {
            if (Constants.gameLogic.shouldSaveData)
            {
                Debug.Log("Saving data ...");

                /* general information */
                SaveGeneralInformation();
                /* item db related */
                SaveItemDB();
                /* main character */
                SaveEntityData(Constants.gameLogic.pc, mainPlayerPath);

                Debug.Log("Finished saving.");
            }
            else
                Debug.Log("No data saved.");
        }

        /***
         * ENTITIES
         ***********/

        int latestCharID;
        HashSet<int> lockedCharIDs;

        public int GetUniqCharID()
        {
            while (lockedCharIDs.Contains(++latestCharID)) { }
            return latestCharID;
        }


        public void SaveEntityData(EntityController ec, int charid)
        {
            SaveEntityData(ec, charXMLPrefix + charid + ".xml");
        }

        public void SaveEntityData(EntityController ec, string path)
        {
            EntitySaveData entData = new EntitySaveData();
            entData.abiList = ec.abiList;
            entData.attrGrpList = ec.attrGrpList;
            entData.attrOtherList = ec.attrOtherList;
            entData.inventory = ec.inventory;
            entData.conversationAssets = ec.interComp.conAssets;
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

        public void LoadEntityData(EntityController ec, int charid)
        {
            LoadEntityData(ec, charXMLPrefix + charid + ".xml");
        }

        // load character data
        public bool LoadEntityData(EntityController ec, string path)
        {
            try
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
                ec.inventory = entSave.inventory;
                ec.interComp.conAssets = entSave.conversationAssets;
                // TODO: read entData here

                if (ReferenceEquals(ec, Constants.gameLogic.pc))
                    Debug.Log("Main Character loaded.");
                else
                    Debug.Log("Entity loaded.");

                return true;
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        /**
         * ITEM DATABASE
         ****************/

        HashSet<int> lockedItemIDs;
        
        public bool ItemIDInUse(int id)
        {
            return lockedItemIDs.Contains(id);
        }

        public void LockItemID(int id)
        {
            Debug.Assert(ItemIDInUse(id), "Cannot lock id [" + id + "] : already locked!");
            lockedItemIDs.Add(id);
        }

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
         * SHOPS
         **********/



        int latestShopID;
        HashSet<int> lockedShopIDs;

        public int GetUniqShopID()
        {
            while (lockedShopIDs.Contains(++latestShopID)) { }
            return latestShopID;
        }

        public bool LoadShop(Shop shop, int shopid)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ShopSaveData));
                FileStream stream = new FileStream(Application.dataPath + shopXMLPrefix + shopid + ".xml", FileMode.Open);
                ShopSaveData sh = serializer.Deserialize(stream) as ShopSaveData;
                stream.Close();

                shop.Set(sh);
                return true;
            } 
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public void SaveShop(Shop shop,int shopid)
        {
            ShopSaveData saveData = new ShopSaveData();
            saveData.shopID = shop.shopID;
            saveData.money = shop.money;
            saveData.items = shop.items;
            saveData.boughtItems = shop.bougthItems;

            XmlSerializer serializer = new XmlSerializer(typeof(ShopSaveData));
            FileStream stream = new FileStream(Application.dataPath + shopXMLPrefix + shopid + ".xml", FileMode.Create);
            serializer.Serialize(stream, saveData); // 
            stream.Close();
        }

        /**
         * GENERAL
         ***********/

        public bool LoadGeneralInformation()
        {
            // load data
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GeneralInformation));
                FileStream stream = new FileStream(Application.dataPath + generalInformationPrefix, FileMode.Open);
                GeneralInformation gi = serializer.Deserialize(stream) as GeneralInformation;
                stream.Close();

                // apply data
                latestShopID = gi.latestShopID;
                latestCharID = gi.latestCharID;
                lockedShopIDs = new HashSet<int>(gi.lockedShopIDs);
                lockedCharIDs = new HashSet<int>(gi.lockedCharIDs);
                lockedItemIDs = new HashSet<int>(gi.lockedItemIDs);
                return true;
            }
            catch (FileNotFoundException e)
            {
                latestCharID = 0;
                latestShopID = 0;
                lockedCharIDs = new HashSet<int>();
                lockedShopIDs = new HashSet<int>();
                lockedItemIDs = new HashSet<int>();
                Debug.LogError(e.Message);
                return false;
            }
        }

        public void SaveGeneralInformation()
        {
            // prepare data
            GeneralInformation gi = new GeneralInformation();
            gi.latestCharID = latestCharID;
            gi.latestShopID = latestShopID;
            gi.lockedCharIDs = new int[lockedCharIDs.Count];
            lockedCharIDs.CopyTo(gi.lockedCharIDs);
            gi.lockedShopIDs = new int[lockedShopIDs.Count];
            lockedShopIDs.CopyTo(gi.lockedShopIDs);
            gi.lockedItemIDs = new int[lockedItemIDs.Count];
            lockedItemIDs.CopyTo(gi.lockedItemIDs);

            // save data
            XmlSerializer serializer = new XmlSerializer(typeof(GeneralInformation));
            FileStream stream = new FileStream(Application.dataPath + generalInformationPrefix, FileMode.Create);
            serializer.Serialize(stream, gi); // 
            stream.Close();
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

    /**
     * DATA CLASSES
     ****************/

    [System.Serializable]
    [XmlRoot("Shop")]
    public class ShopSaveData
    {
        [XmlAttribute("ID")]
        public int shopID;
        public Currency money;
        [XmlArrayItem("i")]
        public List<PraeItem> items;
        [XmlArrayItem("b:")]
        public List<PraeItem> boughtItems;
    }


    [System.Serializable]
    [XmlRoot]
    public class GeneralInformation
    {
        [XmlAttribute]
        public int latestShopID;
        [XmlArrayItem("id")]
        public int[] lockedShopIDs;

        [XmlAttribute]
        public int latestCharID;
        [XmlArrayItem("id")]
        public int[] lockedCharIDs;

        [XmlArrayItem("id")]
        public int[] lockedItemIDs;
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
        [XmlElement("Inventory")]
        public Inventory inventory;
        [XmlArray("conAssets")]
        [XmlArrayItem("c")]
        public List<TextAsset> conversationAssets;
    }
}
