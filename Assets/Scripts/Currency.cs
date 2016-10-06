struct currency 
{
	int _G;
	int _K;
	int _T;
	
	public Currency()
	{
		this(0,0,0);
	}
	
	public Currency(Currency a)
	{
		Set(a.G, a.K, a.T);
	}
	
	public Currency(int g, int k, int t)
	{
		Set(g,k,t);
	}
	
	public static Currency operator +(Currency a)
	{
		return new Currency(_G + a.G, _K + a.K, _T + a.T);
	}
	
	/**
	 * be careful with this one! Currencies must never be negative!
	 */
	public static Currency operator -(Currency a)
	{
		Currency n = new Currency(this); // use this instance as base
		// n.Pay() subtracts a from this instance if possible. It will return TRUE,
		// if subtraction was applied on 'n'. FALSE returns shall throw an error
		Debug.Assert(n.Pay(), "Cannot subtract currencies from another: result would be negative!");
		return n;
	}
	
	// Getter and Setter stuff
	
	public int G
	{
		get { return _G; }
	}
	
	public int K
	{
		get { return _K; }
	}
	
	public int T
	{
		get { return _T; }
	}
	
	public int relative
	{
		get
		{
			return 100*_G + 10*_K + _T;
		}
	}
	
	public static int Relative(int g, int k, int t)
	{
		return 100*g + 10*k + t;
	}
	
	public void Set(Currency a)
	{
		_G = a.G;
		_K = a.K;
		_T = a.T;
	}
	
	public void Set(int g, int k, int t)
	{
		if (g < 0 || k < 0 || t < 0)
			throw new InvalidArgumentEception();
		
		_G = g;
		_K = k;
		_T = t;
	}
	
	public void Add(Currency a)
	{
		_G += a.G;
		_K += a.K;
		_T += a.T;
	}
	
	public void Add(int g, int k, int t)
	{
		if (g < 0 || k < 0 || t < 0)
			throw new InvalidArgumentEception();
		
		_G += g;
		_K += k;
		_T += t;
	}
	
	/**
	 * returns TRUE, if this.relative >= a.relative
	 */
	public bool CanPay(Currency a)
	{
		return this.relative >= a.relative;
	}
	
	/**
	 * remove a from this instance, as long as values do not get negative
	 * @RETURN: FALSE, if the amount a cannot be paid (this.relative < a.relative)
	 */
	public bool Pay(Currency a)
	{
		if (CanPay(a))
		{
			_G = max(_G - a.G, 0);
			a.K += max((a.G - _G)*10, 0); // add the excess if a.G was greater than _G | if (G-a.G) < 0 -> -10*(G-a.G) = 10*(a.G-G)
			_K = max(_K - a.K, 0);
			a.T += max((a.K - _K)*10, 0); // add the excess if a.K was greater than _K
			_T -= a.T;
			Debug.Assert(_T >= 0, "CanPay or Pay calculation invalid!");
			return true;
		}
		else
			return false;
	}
	
	/**
	 * See overloaded version for more details
	 */ 
	public bool Pay(int g, int k, int t)
	{
		return Pay(new Currency(g,k,t));
	}
	
	/**
	 * the values of G, K and T are normalized to that the following constraints are true:
	 * 0 < G
	 * 0 < K < 10
	 * 0 < T < 10
	 * without changing the relative values of this instance.
	 */
	public void Normalize()
	{
		_K += (_T/10)*10;
		_T -= (_T/10)*10; // 3rd constraint applied
		_G += (_K/10)*10;
		_K -= (_K/10)*10; // 2nd and 1st constraint applied
	}
}