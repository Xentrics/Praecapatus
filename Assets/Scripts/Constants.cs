using Assets.Scripts.Managers;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Constants
    {
        /* Global variables which might be accessed throughout different classes */
        public static GameLogic gameLogic;          // fast reference. Set by instances of GameLogic during Awake()
        public static int gameTimeMultiplier = 1;   // might become a use later on
        public static ConsoleManager chatManager;      // fast reference. Set by instances of ChatManager during Awake()
        public static GameObject HUDCanvas;
        public static GameObject StatusUI;
        public static GameObject ShopUI;
        public static GameObject InteractionUI;
        public static InteractionManager interactionManager;

        public static int NUM_ATTRIBUTE_GROUPS = Enum.GetNames(typeof(EAttributeGroup)).Length;
        public static int NUM_ATTRIBUTE_OTHER = Enum.GetNames(typeof(EAttributeOther)).Length;
        public static int NUM_ABILITIES = Enum.GetNames(typeof(Abilities.EAbilities)).Length;

        /**
         * func: disable all UI windows except the one given
         * @m: the UI window to activate
         */
        public static void ActivateUI(EUIMode m)
        {
            StatusUI.SetActive(false);
            ShopUI.SetActive(false);
            InteractionUI.SetActive(false);

            switch(m)
            {
                case EUIMode.NONE:
                    break;
                case EUIMode.INTERACTION_UI:
                    InteractionUI.SetActive(true);
                    break;
                case EUIMode.SHOP_UI:
                    ShopUI.SetActive(true);
                    break;
                case EUIMode.STATUS_UI:
                    StatusUI.SetActive(true);
                    break;
                default:
                    throw new NotImplementedException("UIMode not set yet!");
            }
        }

        public static Vector3 dirToVec(EDirection dir)
        {
            switch(dir)
            {
                case EDirection.EAST:
                    return new Vector3(1,0,0);
                case EDirection.WEST:
                    return new Vector3(-1,0,0);
                case EDirection.NORTH:
                    return new Vector3(0,-1,0);
                case EDirection.SOUTH:
                    return new Vector3(0,1,0);
                default:
                    return new Vector3();
            }
        }
    }

    public enum EUIMode
    {
        NONE,
        STATUS_UI,
        INTERACTION_UI,
        SHOP_UI
    }
}
