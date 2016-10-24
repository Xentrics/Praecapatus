using UnityEngine;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class PraeGear : PraeItem
    {
        public EGearType gearType = EGearType.UNDEFINED;


        public PraeGear() : base() { }
        /**
         * create items outside the database, if necessary
         * stacksize should typically be one, except for stuff like ammunition
         */
        public PraeGear(string name, float weightSingle, Currency value, int amount, int stackSize, Sprite icon) : base(name, weightSingle, value, amount, stackSize, icon)
        {
            
        }

        public PraeGear(string name, float weightSingle, Currency value, Sprite icon) : this(name, weightSingle, value, 1, 1, icon) {}
    }


    public enum EGearType
    {
        UNDEFINED,
        ARMOR_HEAD,
        ARMOR_TORSO,
        ARMOR_ARMS,
        ARMOR_LEGS,
        ARMOR_SHOES,
        WEAPON_SINGLE,
        WEAPON_DOUBLE,
        WEAPON_BOW,
        WEAPON_SHIELD
    }
}
