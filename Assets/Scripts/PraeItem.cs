public class PraeItem
{
	string name;
	float _weightSingle;
	Currency value;
	int _amount;
	int _stackSize;
	Icon _icon;
	
	/**
	 * func: 	add 'am' to the amount of this item
	 *			this method does not throw anz exceptions
	 * @am 		the amount to add to this item
	 * @return 	the amount that could not fit based on the current and the maximum stack size
	 */
	public int Add(int am)
	{
		if (_amount + am <= _stackSize)
		{
			_amount += am;
			return 0;
		}
		else
		{
			// am cannot be added: maximum stack size reached
			am = _stackSize - (_amount + am); // use am as return value
			_amount = _stackSize;
			return am;
		}
	}
	
	public float weight
	{
		get 
		{
			return _weightSingle * _amount;
		}
		
		// NOTE: one may not make a setter for the total weight. use weightSingle instead!
	}
	
	public float weightSingle
	{
		get 
		{
			return _weightSingle;
		}
		
		set
		{
			if (value < 0)
				throw new ArgumentOutOfBoundsException();
			else
				_weightSingle = value;
		}
	}
	
	public int stackSize
	{
		get { return _stackSize; }
		set
		{
			if (value < 0)
				throw new ArgumentOutOfBoundsException();
			else
				_stackSize = value;
		}
	}
	
	public int amount
	{
		get { return _amount; }
		set 
		{
			if (value < 0)
				throw new ArgumentOutOfBoundsException();
			else if (value > _stackSize)
				throw new ItemStackException("Item amount to add exceeds maximum stack size!");
			else
				_amount = value;
		}
	}
}