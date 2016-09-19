using System;

namespace Assets.Scripts
{
    public static class Constants
    {
        public static GameLogic gameLogic; // fast reference. Set by instances of GameLogic during Awake()
        public static int gameTimeMultiplier = 1;   // might become a use later on

        public static int NUM_ATTRIBUTE_GROUPS = Enum.GetNames(typeof(EAttributeGroup)).Length;
        public static int NUM_ATTRIBUTE_OTHER = Enum.GetNames(typeof(EAttributeOther)).Length;

        public static UnityEngine.Vector3 dirToVec(EDirection dir)
        {
            switch(dir)
            {
                case EDirection.EAST:
                    return new UnityEngine.Vector3(1,0,0);
                case EDirection.WEST:
                    return new UnityEngine.Vector3(-1,0,0);
                case EDirection.NORTH:
                    return new UnityEngine.Vector3(0,-1,0);
                case EDirection.SOUTH:
                    return new UnityEngine.Vector3(0,1,0);
                default:
                    return new UnityEngine.Vector3();
            }
        }
    }
}
