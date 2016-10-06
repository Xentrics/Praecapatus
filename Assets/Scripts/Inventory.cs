public class Inventory
{
	float _weight;
	float _maxWeight;
	Currency _money;
	List<PraeItem> items;
	
	public PraeItem GetItem(int id)
	{
		if (id < 0 || id >= items.Count)
			throw new ArgumentOutOfBoundsException();
		else
			return items[id];
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
			// stack items
		}
		else
		{
			// just check if the weight limit is exceeded
			if (addPartial)
			{
				// add as much as possible
				if (item.weight + _weight > _maxWeight)
				{
					int weightDiff = item.weight + _weight - _maxWeight;
					int fitAmount = (int) (weightDiff / item.weightSingle);
					itemCopy.amount = fitAmount;
					items.Add(itemCopy);
					_weight += itemCopy.weight;
				}
				else
				{
					items.Add(itemCopy);
					_weight += item.weight;
				}
			}
			else
			{
				// only add stuff if enough space and weight available
				if (item.weight + _weight > _maxWeight)
					return item.amount;
				else
				{
					items.Add(itemCopy);
					_weight += item.weight;
				}
			}
		}
	}
	
	public void RemoveItem(int id)
	{
		if (id < 0 || id >= items.Count)
			throw new ArgumentOutOfBoundsException();
		else
			items.RemoveAt(id);
	}
	
	pubblic bool RemoveItem(PraeItem d)
	{
		throw new NotImplementedEception();
	}
}