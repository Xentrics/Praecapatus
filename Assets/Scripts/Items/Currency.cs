using System.Xml.Serialization;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public struct Currency
    {
        [UnityEngine.SerializeField] int _G;
        [UnityEngine.SerializeField] int _K;
        [UnityEngine.SerializeField] int _T;

        public Currency(Currency a) : this()
        {
            Set(a);
        }

        public Currency(int g, int k, int t) : this()
        {
            Set(g, k, t);
        }

        /**
         * return the sum of a + b as a new currency instance
         */
        public static Currency operator +(Currency a, Currency b)
        {
            return new Currency(a._G + b.G, a._K + b.K, a._T + b.T);
        }

        /**
         * be careful with this one! Currencies must never be negative!
         * return a - b as a new currency instance as long as a - b < 0 is false
         */
        public static Currency operator -(Currency a, Currency b)
        {
            Currency a_copy = new Currency(a); // use this instance as base
                                               // n.Pay() subtracts a from this instance if possible. It will return TRUE,
                                               // if subtraction was applied on 'n'. FALSE returns shall throw an error
            System.Diagnostics.Debug.Assert(a_copy.Pay(b), "Cannot subtract currencies from another: result would be negative!");
            return a_copy;
        }

        /** Getter and Setter stuff **/

        [XmlAttribute("G")]
        public int G
        {
            get { return _G; }
            set
            {
                if (value < 0)
                    UnityEngine.Debug.LogError("Currencies (G) must not be negative!");
                _G = value;
            }
        }

        [XmlAttribute("K")]
        public int K
        {
            get { return _K; }
            set
            {
                if (value < 0)
                    UnityEngine.Debug.LogError("Currencies (K) must not be negative!");
                _K = value;
            }
        }

        [XmlAttribute("T")]
        public int T
        {
            get { return _T; }
            set
            {
                if (value < 0)
                    UnityEngine.Debug.LogError("Currencies (T) must not be negative!");
                _T = value;
            }
        }

        /**
         * return the relative value of this currency as single int
         * relative := 100 * G + 10 * K + T
         */
        [XmlIgnore]
        public int relative
        {
            get { return 100 * _G + 10 * _K + _T; }
            set { Normalize(value); }
        }

        /**
         * return the relative value of this currency as single int
         * relative := 100 * G + 10 * K + T
         */
        public static int Relative(int g, int k, int t)
        {
            return 100 * g + 10 * k + t;
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
                throw new System.ArgumentOutOfRangeException();

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
                throw new System.ArgumentOutOfRangeException();

            _G += g;
            _K += k;
            _T += t;
        }

        /**
         * returns TRUE, if this.relative >= a.relative
         */
        public bool CanPay(Currency a)
        {
            return relative >= a.relative;
        }

        /**
         * remove a from this instance, as long as values do not get negative
         * @RETURN: FALSE, if the amount a cannot be paid (this.relative < a.relative)
         */
        public bool Pay(Currency a)
        {
            if (CanPay(a))
            {
                /*
                _G = System.Math.Max(_G - a.G, 0);
                a.K += System.Math.Max((a.G - _G) * 10, 0); // add the excess if a.G was greater than _G | if (G-a.G) < 0 -> -10*(G-a.G) = 10*(a.G-G)
                _K = System.Math.Max(_K - a.K, 0);
                a.T += System.Math.Max((a.K - _K) * 10, 0); // add the excess if a.K was greater than _K
                _T -= a.T;
                System.Diagnostics.Debug.Assert(_T >= 0, "CanPay or Pay calculation invalid!");
                */
                // partition based on relative
                relative -= a.relative;
                UnityEngine.Debug.Assert(relative >= 0, "something went wring during pay calculation!");

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
            return Pay(new Currency(g, k, t));
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
            Normalize(relative);
        }

        public void Normalize(int newRelative)
        {
            _G = newRelative / 100;
            newRelative -= _G * 100;
            _K = newRelative / 10;
            newRelative -= _K * 10;
            _T = newRelative;
            UnityEngine.Debug.Assert(_T >= 0 && _T < 10 && _K >= 0 && _K < 10 && _G >= 0, "currency normalization failed!");
        }

        public bool SanityCheck()
        {
            return (_G >= 0 && _K >= 0 && _T >= 0) ? true : false;
        }

        public bool Equals(Currency c)
        {
            return _G == c._G && _K == c._K && _T == c._T;
        }
    }
}