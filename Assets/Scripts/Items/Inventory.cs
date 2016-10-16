using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class Inventory
    {
        [UnityEngine.SerializeField] float _weight;
        [UnityEngine.SerializeField] float _maxWeight = 20f;
        [UnityEngine.SerializeField] Currency _money = new Currency();
        [UnityEngine.SerializeField] List<PraeItem> _items = new List<PraeItem>();

        public Inventory()
        {
            UnityEngine.Debug.Log("inv!");
        }

        public PraeItem GetItem(int id)
        {
            if (id < 0 || id >= _items.Count)
                throw new System.ArgumentOutOfRangeException();
            else
                return _items[id];
        }

        /**
         * 
         */
        private int AddPartial(PraeItem itemCopy)
        {
            if (itemCopy.weight + _weight > _maxWeight)
            {
                float weightDiff = itemCopy.weight + _weight - _maxWeight;
                int fitAmount = (int)(weightDiff / itemCopy.weightSingle); // the amount of the item that can still be added
                if (fitAmount > 0)
                {
                    itemCopy.amount = fitAmount;
                    _items.Add(itemCopy);
                    _weight += itemCopy.weight;
                    return itemCopy.amount - fitAmount;
                }
                else
                    return itemCopy.amount;
            }
            else
            {
                // item fits completely
                _items.Add(itemCopy);
                _weight += itemCopy.weight;
                return 0;
            }
        }

        /**
         * @item 	the item being added to this inventory
         * @stack	TRUE: try to find items of the same type as 'item' and add the item by increasing
         *			'amount' attribute of the items found in this inventory
         * @addPartial TRUE: 	add 'item' even if it cannot fit completly in this inventory
         *						return will be greater than 0 if item could not fit
         * @return	the amount of 'item' that could not be fit into this inventory by having either
         *			not enough space left or the inventory getting to heavy (weight > maxweight)
         */
        public int AddItem(PraeItem item, bool stack = false, bool addPartial = true)
        {
            PraeItem itemCopy = new PraeItem(item); // decouple references

            if (stack)
            {
                if (addPartial || (itemCopy.weight + _weight <= _maxWeight))
                {
                    /* iterate all items and add 'item' until its amount is zero or weight constraint is reached */
                    int rest = itemCopy.amount;
                    foreach (PraeItem i in _items)
                    {
                        // item names must match and stacksize not reached
                        if (i.name.Equals(itemCopy.name) && i.amount < i.stackSize)
                        {
                            if (i.stackSize - i.amount > rest)
                            {
                                // rest amount fits
                                i.amount += rest;
                                return 0;
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
                        return AddPartial(itemCopy);
                    }
                    else
                        return 0;
                }
                else
                {
                    // cannot fit
                    return itemCopy.amount;
                }
            }

            else

            {
                // items will be added in new slot
                if (addPartial)
                {
                    /* add as much as can still fit */
                    return AddPartial(itemCopy);
                }
                else
                {
                    /* only add stuff if enough space and weight available */
                    if (itemCopy.weight + _weight > _maxWeight)
                        return itemCopy.amount;
                    else
                    {
                        // item fits completely
                        _items.Add(itemCopy);
                        _weight += itemCopy.weight;
                        return 0;
                    }
                }
            }
        }

        public void RemoveItem(int id)
        {
            if (id < 0 || id >= _items.Count)
                throw new System.ArgumentOutOfRangeException();
            else
                _items.RemoveAt(id);
        }

        public bool RemoveItem(PraeItem d)
        {
            throw new System.NotImplementedException();
        }

        /**
         * GETTER AND SETTER
         *********************/

        public void Set(Inventory inv)
        {
            _weight = inv.weight;
            _maxWeight = inv.maxWeight;
            _money = inv.currency;
            _items = inv._items;

        }

        public Currency currency
        {
            get { return _money; }
            set
            {
                if (value != null)
                    _money.Set(value);
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

        public float weight
        {
            get { return _weight; }
            set
            {
                if (value < 0)
                    UnityEngine.Debug.LogError("Cannot set inventory weight: value is negative!");
                _weight = value;
            }
        }

        public float maxWeight
        {
            get { return _maxWeight; }
            set
            {
                if (value < 0)
                    UnityEngine.Debug.LogError("Cannot set inventory maxWeight: value is negative!");
                _maxWeight = value;
            }
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
                    UnityEngine.Debug.LogError("Inventory items set to NULL!");
                else
                    _items = value;
            }
        }

        public override string ToString()
        {
            string str = "weight: [" + _weight + "/" + _maxWeight + "\n";
            foreach (PraeItem i in _items)
                str = str + i.ToString();

            return str;
        }
    }
}