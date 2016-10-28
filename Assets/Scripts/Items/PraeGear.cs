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
        public PraeGear(string name, float weightSingle, Currency value, Sprite icon, EGearType gearType, bool sellable = true, int amount = 1, int stackSize = 1) 
            : base(name, weightSingle, value, icon, sellable, amount, stackSize)
        {
            this.gearType = gearType;
        }


        public void Set(PraeGear pg)
        {
            base.Set(pg);
            gearType = pg.gearType;
        }
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
