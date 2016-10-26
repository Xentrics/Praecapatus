using UnityEngine;
using System.Xml.Serialization;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    [XmlRoot("Equipment")]
    public class Equipment
    {
        [SerializeField][XmlElement("Head")]  public PraeArmor armor_head;
        [SerializeField][XmlElement("Torso")] public PraeArmor armor_torso;
        [SerializeField][XmlElement("Arms")]  public PraeArmor armor_arms;
        [SerializeField][XmlElement("Legs")]  public PraeArmor armor_legs;
        [SerializeField][XmlElement("Shoes")] public PraeArmor armor_shoes;

        [SerializeField][XmlElement("LeftHand")]  public PraeWeapon weapon_lefthand;
        [SerializeField][XmlElement("RightHand")] public PraeWeapon weapon_righthand;

        public void Set(Equipment eq)
        {
            armor_head.Set(eq.armor_head);
            armor_torso.Set(eq.armor_torso);
            armor_arms.Set(eq.armor_arms);
            armor_legs.Set(eq.armor_legs);
            armor_shoes.Set(eq.armor_shoes);

            weapon_lefthand.Set(eq.weapon_lefthand);
            weapon_righthand.Set(eq.weapon_righthand);
        }
    }
}
