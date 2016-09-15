using System;

namespace Assets.Scripts
{
    public static class Constants
    {
        public static int NUM_ATTRIBUTES = Enum.GetNames(typeof(EAttrGrp)).Length;

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
