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
        public bool shouldSave;
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
            if (shouldSave)
            {
                Debug.Log("Saving data ...");
                /* item db related */
                SaveItemDB();
                /* main character */
                SaveEntityData(Constants.gameLogic.pc, mainPlayerPath);
                /* general information - ALWAYS SAVE LAST */
                SaveGeneralInformation();

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

            if (ReferenceEquals(ec, Constants.gameLogic.pc))
                Debug.Log("Main Character saved.");
            else
                Debug.Log("Entity saved.");
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

        int latestItemID;
        HashSet<int> lockedItemIDs;
        
        public int GetNewItemID()
        {
            while (lockedItemIDs.Contains(++latestItemID)) { }
            LockItemID(latestItemID);
            return latestItemID;
        }

        public bool IsItemIDInUse(int id)
        {
            return lockedItemIDs.Contains(id);
        }

        public void LockItemID(int id)
        {
            Debug.Assert(!IsItemIDInUse(id), "Cannot lock id [" + id + "] : already locked!");
            lockedItemIDs.Add(id);
        }

        public PraeItem GetItemById(int id)
        {
            PraeItem item;
            itemDB.itemDic.TryGetValue(id, out item);
            if (item != null)
                return item;
            else
            {
                Debug.LogError("Couldn't find item with id [" + id + "] in databse!");
                return null;
            }
        }

        public void AddToItemDB(PraeItem i)
        {
            if (i == null)
                throw new System.NullReferenceException("cannot add null to item database");

            if (i.id <= 0)
                i.id = GetNewItemID();
            itemDB.items.Add(i);
        }

        /**
         * working in unity editor allows adding items to the database without checkups
         * cleanup here
         * is called during 'SaveItemDB' when working in the editor
         */
        private void EnforceItemIds()
        {
            foreach (PraeItem i in itemDB.items)
                if (i.id <= 0)
                    i.id = GetNewItemID();
            itemDB.items.Sort((x,y)=> { return x.id.CompareTo(y.id); });
        }

        public void SaveItemDB()
        {
#if UNITY_EDITOR
            EnforceItemIds();
            if (!SanityCheck())
                Debug.LogError("One or more items have invalid properties!");
#endif
            // open a new xml file
            XmlSerializer serializer = new XmlSerializer(typeof(ItemDB));
            FileStream stream = new FileStream(Application.dataPath + itemDBPath, FileMode.Create);
            serializer.Serialize(stream, itemDB); // 
            stream.Close();

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

            // make dictionary for instant access
            itemDB.itemDic = new Dictionary<int, PraeItem>(itemDB.items.Count+10);
            foreach (PraeItem item in itemDB.items)
            {
                itemDB.itemDic.Add(item.id, item);
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

        int latestConId;
        [SerializeField] List<UIdToConData> idToConNameList;

        public string GetConNameById(int id)
        {
            int i = id;
            while(i >= 0 && idToConNameList[i].id != id) { --i; }

            if (idToConNameList[i].id == id)
                return idToConNameList[i].conName;
            else
            {
                Debug.Log("Could not find conversation with id: " + id);
                return "";
            }
        }
        
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
                latestItemID = gi.latestItemID;
                latestConId = gi.latestConID;
                idToConNameList = new List<UIdToConData>(gi.idToconData);
                lockedShopIDs = new HashSet<int>(gi.lockedShopIDs);
                lockedCharIDs = new HashSet<int>(gi.lockedCharIDs);
                lockedItemIDs = new HashSet<int>(gi.lockedItemIDs);
                return true;
            }
            catch (FileNotFoundException e)
            {
                latestCharID = 1;
                latestShopID = 1;
                latestItemID = 1;
                latestConId = 1;
                lockedCharIDs = new HashSet<int>();
                lockedShopIDs = new HashSet<int>();
                lockedItemIDs = new HashSet<int>();
                idToConNameList = new List<UIdToConData>();
                Debug.LogError(e.Message);
                return false;
            }
        }

        void preventIdGaps(int[] arr, out int latestIndex, bool sort = true)
        {
            if (sort)
                System.Array.Sort(arr);

            if (arr[0] != 1)
                latestConId = 1;
            for (int i=1; i<arr.Length; ++i)
                if (arr[i-1] + 1 != arr[i])
                {
                    latestIndex = i + 1;
                    return;
                }

            latestIndex = arr[arr.Length - 1];
        }

        void preventIdGaps(List<UIdToConData> arr, out int latestIndex)
        {
            if (arr[0].id != 1)
                latestConId = 1;
            for (int i = 1; i < arr.Count; ++i)
                if (arr[i - 1].id + 1 != arr[i].id)
                {
                    latestIndex = i + 1;
                    return;
                }

            latestIndex = arr[arr.Count - 1].id;
        }

        public void SaveGeneralInformation()
        {
            // prepare data
            GeneralInformation gi = new GeneralInformation();

            gi.lockedCharIDs = new int[lockedCharIDs.Count];
            lockedCharIDs.CopyTo(gi.lockedCharIDs);
            gi.lockedShopIDs = new int[lockedShopIDs.Count];
            lockedShopIDs.CopyTo(gi.lockedShopIDs);
            gi.lockedItemIDs = new int[lockedItemIDs.Count];
            lockedItemIDs.CopyTo(gi.lockedItemIDs);

            // handle id gaps and so on
#if UNITY_EDITOR
            idToConNameList.Sort((x, y) => { return x.id.CompareTo(y.id); });
            preventIdGaps(idToConNameList, out gi.latestConID);
            preventIdGaps(gi.lockedCharIDs, out gi.latestCharID);
            preventIdGaps(gi.lockedItemIDs, out gi.latestItemID);
            preventIdGaps(gi.lockedShopIDs, out gi.latestShopID);
#endif
            gi.idToconData = idToConNameList.ToArray();

            gi.latestCharID = latestCharID;
            gi.latestShopID = latestShopID;
            gi.latestItemID = latestItemID;
            gi.latestConID = latestConId;

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
            Dictionary<int, PraeItem> ItemIDMap = new Dictionary<int, PraeItem>(itemDB.items.Count+1); // any 2 items must not share the same id!
            foreach (PraeItem i in itemDB.items)
            {
                if (!i.SanityCheck())
                {
                    Debug.LogError("Invalid item properties of item: " + i.name);
                    failed = true;
                }

                if (ItemIDMap.ContainsKey(i.id))
                {
                    Debug.LogError("Item [" + ItemIDMap[i.id].name + "] and Item [" + i.name + "] share same id!");
                    i.id = GetNewItemID();
                    Debug.LogError("ID shifted to: " + i.id);
                    failed = true;
                }

                ItemIDMap.Add(i.id, i);
            }

            return !failed;
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
    public struct UIdToConData
    {
        [XmlAttribute] public int id;
        [XmlAttribute] public string conName;

        public UIdToConData(int id, string conName)
        {
            this.id = id;
            this.conName = conName;
        }
    }

    [System.Serializable]
    [XmlRoot]
    public class GeneralInformation
    {
        [XmlAttribute]
        public int latestShopID;
        [XmlArrayItem("s")]
        public int[] lockedShopIDs;

        [XmlAttribute]
        public int latestCharID;
        [XmlArrayItem("c")]
        public int[] lockedCharIDs;

        [XmlAttribute] 
        public int latestItemID;
        [XmlArrayItem("i")]
        public int[] lockedItemIDs;

        [XmlAttribute]
        public int latestConID;
        [XmlArrayItem("c")]
        public UIdToConData[] idToconData;
    }

    [System.Serializable]
    [XmlRoot("ItemCollection")]
    public class ItemDB
    {
        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<PraeItem> items = new List<PraeItem>();
        [XmlIgnore]
        public Dictionary<int, PraeItem> itemDic;
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
