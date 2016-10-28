using UnityEngine;
using System.Xml.Serialization;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class PraeArmor : PraeGear
    {
        [SerializeField] protected float _armor;

        public PraeArmor() : base() { }

        public PraeArmor(string name, float weightSingle, Currency value, Sprite icon, EGearType gearType, bool sellable = true, int amount = 1, int stackSize = 1) 
            : base(name, weightSingle, value, icon, gearType, sellable, amount, stackSize)
        {

        }


        /**
         * GETTER AND SETTER
         *********************/

        public void Set(PraeArmor ar)
        {
            base.Set(ar);
            _armor = ar._armor;
        }

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
