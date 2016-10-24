using UnityEngine;
using System.Xml.Serialization;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class PraeArmor : PraeGear
    {
        [SerializeField] protected float _armor;

        public PraeArmor() : base() { }

        public PraeArmor(string name, float weightSingle, Currency value, int amount, int stackSize, Sprite icon) : base(name, weightSingle, value, amount, stackSize, icon)
        {

        }

        public PraeArmor(string name, float weightSingle, Currency value, Sprite icon) : this(name, weightSingle, value, 1, 1, icon) { }


        /**
         * GETTER AND SETTER
         *********************/

        [XmlAttribute("armor")]
        public float armor
        {
            get { return _armor; }
            set
            {
                if (value < 0)
                    Debug.LogError("armor must not be negative!");
                else
                    _armor = 0;
            }
        }
    }
}
