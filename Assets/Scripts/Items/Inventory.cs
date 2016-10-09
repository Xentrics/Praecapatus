using System.Collections.Generic;

namespace Assets.Scripts.Items
{
    public class Inventory
    {
        float _weight;
        float _maxWeight;
        Items.Currency _money;
        List<PraeItem> items;

        public PraeItem GetItem(int id)
        {
            if (id < 0 || id >= items.Count)
                throw new System.ArgumentOutOfRangeException();
            else
                return items[id];
        }

        /**
         * 
         */
        private int AddPartial(ref PraeItem itemCopy)
        {
            if (itemCopy.weight + _weight > _maxWeight)
            {
                float weightDiff = itemCopy.weight + _weight - _maxWeight;
                int fitAmount = (int)(weightDiff / itemCopy.weightSingle); // the amount of the item that can still be added
                if (fitAmount > 0)
                {
                    itemCopy.amount = fitAmount;
                    items.Add(itemCopy);
                    _weight += itemCopy.weight;
                    return itemCopy.amount - fitAmount;
                }
                else
                    return itemCopy.amount;
            }
            else
            {
                // item fits completely
                items.Add(itemCopy);
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
                if (addPartial || (itemCopy.weight + _weight < _maxWeight))
                {
                    /* iterate all items and add 'item' until its amount is zero or weight constraint is reached */
                    int rest = itemCopy.amount;
                    foreach (PraeItem i in items)
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
                        return AddPartial(ref itemCopy);
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
                    return AddPartial(ref itemCopy);
                }
                else
                {
                    /* only add stuff if enough space and weight available */
                    if (itemCopy.weight + _weight > _maxWeight)
                        return itemCopy.amount;
                    else
                    {
                        // item fits completely
                        items.Add(itemCopy);
                        _weight += itemCopy.weight;
                        return 0;
                    }
                }
            }
        }

        public void RemoveItem(int id)
        {
            if (id < 0 || id >= items.Count)
                throw new System.ArgumentOutOfRangeException();
            else
                items.RemoveAt(id);
        }

        public bool RemoveItem(PraeItem d)
        {
            throw new System.NotImplementedException();
        }
    }
}