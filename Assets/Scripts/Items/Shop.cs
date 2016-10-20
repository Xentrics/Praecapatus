using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [Serializable]
    public class Shop : MonoBehaviour
    {
        [SerializeField] protected int _shopID = -1;
        [SerializeField] Currency _money;
        [SerializeField] List<PraeItem> _items = new List<PraeItem>();
        [SerializeField] List<PraeItem> _boughtItems = new List<PraeItem>();

        void Awake()
        {
        }

        void Start()
        {
            if (_shopID > -1)
                Constants.xmlHandler.LoadShop(this, _shopID);
            else
                /*TODO: GetUniqShopID? */;
        }

        void OnApplicationQuit()
        {
            if (Constants.gameLogic.shouldSaveData)
            {
                if (_shopID == -1)
                    Debug.LogError("ERROR: cannot save shop without valid id!");
                else
                {
                    Constants.xmlHandler.SaveShop(this, _shopID);
                }
            }
        }

        public void Register()
        {
            if (_shopID == -1)
            {
                _shopID = Constants.xmlHandler.GetUniqShopID();
                Debug.Log("New Shop registered: " + _shopID);
            }
        }

        public void AddBoughtItem(PraeItem item)
        {
            _boughtItems.Add(item);
        }

        public void AddItem(PraeItem item)
        {
            PraeItem itemCopy = new PraeItem(item); // decouple references

            /* iterate all items and add 'item' until its amount is zero or weight constraint is reached */
            int rest = itemCopy.amount;
            foreach (PraeItem i in _items)
            {
                // item names must match and stacksize not reached
                if (i.Equals(itemCopy) && i.amount < i.stackSize)
                {
                    if (i.stackSize - i.amount > rest)
                    {
                        // rest amount fits
                        i.amount += rest;
                        return;
                    }
                    else
                    {
                        // i can only take some of our item
                        rest -= (i.stackSize - i.amount);
                        i.amount = i.stackSize;
                    }
                }
            }

            if (rest > 0)
            {
                // all items in inventory checked. Add a new item, if weight constraint not violated
                itemCopy.amount = rest;
                _items.Add(itemCopy);
            }
        }

        /**
         * GETTER AND SETTER
         *********************/

        public void RemoveItem(int id)
        {
            if (id < 0 || id >= _items.Count)
                throw new ArgumentOutOfRangeException();
            else
                _items.RemoveAt(id);
        }

        public bool RemoveItem(PraeItem d)
        {
            return _items.Remove(d);
        }

        public void RemoveBoughtItem(int id)
        {
            if (id >= 0 && id < _boughtItems.Count)
                _boughtItems.RemoveAt(id);
            else
                throw new ArgumentOutOfRangeException("Cannot remove item at postion invalid index!");
        }

        public void Set(Shop sh)
        {
            _money = sh._money;
            _items = sh._items;
            _boughtItems = sh._boughtItems;
            _shopID = sh._shopID;
        }

        public void Set(Managers.ShopSaveData sh)
        {
            _money = sh.money;
            _items = sh.items;
            _boughtItems = sh.boughtItems;
            _shopID = sh.shopID;
        }

        public void Set(Inventory inv)
        {
            _money = inv.money;
            _items = inv.items;
        }

        public Currency money
        {
            get { return _money; }
            set
            {
                if (value != null)
                    _money.Set(value);
            }
        }

        public int shopID
        {
            get { return _shopID; }
            set
            {
                if (value > 0)
                    _shopID = value;
                else
                    Debug.LogError("ShopID set to negative value. INVALID!");
            }
        }

        public int G
        {
            get { return _money.G; }
        }

        public int K
        {
            get { return _money.K; }
        }

        public int T
        {
            get { return _money.T; }
        }

        public int boughtItemCount
        {
            get { return _boughtItems.Count; }
        }

        public List<PraeItem> bougthItems
        {
            get { return _boughtItems; }
            protected set { _boughtItems = value; }
        }

        public int itemCount
        {
            get { return items.Count; }
        }

        public List<PraeItem> items
        {
            get { return _items; }
            protected set
            {
                if (value == null)
                    Debug.LogError("Inventory items set to NULL!");
                else
                    _items = value;
            }
        }

        public override string ToString()
        {
            string str = "items: \n";
            foreach (PraeItem i in _items)
                str = str + i.ToString();

            return str;
        }
    }
}
