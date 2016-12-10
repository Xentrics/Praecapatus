using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class PraeItem
    {
        public static int GENERATED = 0;

        [SerializeField]        protected int       _id;    // is only important to identify specific items (e.g. quest items, template items). Default 0 is considered "unset"
        [XmlAttribute("name")]  public string       name;
        [XmlElement]            public string       desc;
        [SerializeField]        protected float     _weightSingle;
        [SerializeField]        protected int       _amount;
        [SerializeField]        protected int       _stackSize;
        [XmlAttribute]          public bool         sellable = true;
        [XmlElement]            public Currency     value;
        [SerializeField]        protected Sprite    _icon;
        [SerializeField]        protected TextAsset _quest;

        public PraeItem() {}

        public PraeItem(PraeItem copy)
        {
            Set(copy);
        }

        public PraeItem(PraeItem copy, int newAmount) : this(copy)
        {
            _amount = newAmount;
        }

        /**
         * create items outside the database, if necessary
         */
        public PraeItem(string name, float weightSingle, Currency value, Sprite icon, bool sellable = true, int amount = 1, int stackSize = 1)
        {
            this.name = name;
            this._weightSingle = weightSingle;
            this.value = value;
            this._amount = amount;
            this._stackSize = stackSize;
            this.sellable = sellable;
            this._icon = (icon) ? icon : Managers.XmlDBManager.NotSetIcon;
        }


        public virtual void Set(PraeItem it)
        {
            _id = it._id;
            name = it.name;
            desc = it.desc;
            _weightSingle = it._weightSingle;
            value = it.value;
            _amount = it._amount;
            _stackSize = it._stackSize;
            sellable = it.sellable;
            _icon = it._icon;
        }


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

        [XmlAttribute("amount")]
        public int amount
        {
            get { return _amount; }
            set
            {
                if (value < 0)
                    Debug.LogError("Item " + name + " : amount negative!");
                else if (_stackSize > 0 && value > _stackSize)
                    Debug.LogError("Item " + name + " : amount exceeds stackSize!");
                _amount = value;
            }
        }

        [XmlIgnore]
        public Sprite icon
        {
            get { return _icon; }
            set
            {
                _icon = (value) ? value : Managers.XmlDBManager.NotSetIcon;
            }
        }

        [XmlAttribute]
        public int id
        {
            get { return _id; }
            set
            {
                if (value < 0)
                    Debug.LogError("[" + name + "] id set to negative value. Intended?");
                _id = value;
            }
        }

        /**
         * just for XML loading/saving
         */
        [XmlElement]
        public string iconPath
        {
            get { return UnityEditor.AssetDatabase.GetAssetPath(_icon); }

            protected set
            {
                // HACK: i load icon here instead of setting iconpath
                //       this is just to make XML load and save this stuff automatically
                if (value != null && value.Trim() != "")
                {
                    Debug.Log("iconPath: <" + value.Trim() + ">.");
                    Sprite s = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath(value, typeof(Sprite)); // HACK: best way to load assets!
                    icon = s;
                    if (!s)
                        Debug.LogError("Item " + value + " : was not found at given path!");
                }
                else
                    icon = null;
            }
        }

        [XmlElement]
        public string quest
        {
            get { return _quest.name; }
            protected set
            {
                if (value != null && value.Trim() != "")
                {
                    TextAsset t = (TextAsset)UnityEditor.AssetDatabase.LoadAssetAtPath(value, typeof(TextAsset));
                    _quest = t;
                    if (!t)
                        Debug.LogError("Quest " + value + " : was not found at given path!");
                }
                else
                    _quest = null;
            }
        }

        [XmlAttribute("stackSize")]
        public int stackSize
        {
            get { return _stackSize; }
            protected set
            {
                if (value <= 0 || value < _amount)
                    Debug.LogError("Item " + name + " : stackSize negative,zero or lower than amount!");
                _stackSize = value;
            }
        }


        [XmlIgnore]
        public float weight
        {
            get
            {
                return _weightSingle * _amount;
            }

            // NOTE: one may not make a setter for the total weight. use weightSingle instead!
        }

        [XmlAttribute("weight")]
        public float weightSingle
        {
            get
            {
                return _weightSingle;
            }

            protected set
            {
                if (value < 0)
                    Debug.LogError("Item " + name + " : weight negative!");
                _weightSingle = value;
            }
        }

        public bool SanityCheck()
        {
            return (name != null && value.SanityCheck() && _amount >= 0 && _amount <= stackSize && _stackSize > 0 && _weightSingle >= 0) ? true : false;
        }

        public bool Equals(PraeItem i)
        {
            return name.Equals(i.name) &&
                desc.Equals(i.desc) &&
                _weightSingle == i._weightSingle &&
                _stackSize == i._stackSize &&
                value.Equals(i.value);
                _quest.Equals(i._quest);
            // TODO: icon equals?
        }

        public override string ToString()
        {
            return "ITEM: " + name + " [" + _amount + "/" + _stackSize + "]";
        }
    }
}