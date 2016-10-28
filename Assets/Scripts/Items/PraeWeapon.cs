using UnityEngine;
using System.Xml.Serialization;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class PraeWeapon : PraeGear
    {
        [SerializeField] protected float _damage;

        public PraeWeapon() : base() { }

        public PraeWeapon(string name, float weightSingle, Currency value, Sprite icon, EGearType gearType, bool sellable = true, int amount = 1, int stackSize = 1) 
            : base(name, weightSingle, value, icon, gearType, sellable, amount, stackSize)
        {

        }

        /**
         * GETTER AND SETTER
         *********************/

        public void Set(PraeWeapon pw)
        {
            base.Set(pw);

            _damage = pw._damage;
        }

        [XmlAttribute("damage")]
        public float damage
        {
            get { return _damage; }
            set
            {
                if (value < 0)
                    Debug.LogError("Damage of weapons cannot be negative!");
                else
                    _damage = value;
            }
        }
    }
}
