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
    }
}
