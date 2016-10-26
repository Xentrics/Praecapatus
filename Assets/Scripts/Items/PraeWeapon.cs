using UnityEngine;
using System.Xml.Serialization;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class PraeWeapon : PraeGear
    {
        [SerializeField] protected float _damage;

        public PraeWeapon() : base() { }

        public PraeWeapon(string name, float weightSingle, Currency value, int amount, int stackSize, Sprite icon) : base(name, weightSingle, value, amount, stackSize, icon)
        {

        }

        public PraeWeapon(string name, float weightSingle, Currency value, Sprite icon) : this(name, weightSingle, value, 1, 1, icon) { }

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
