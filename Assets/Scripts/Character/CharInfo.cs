﻿using UnityEngine;

namespace Assets.Scripts
{
    class CharInfo : MonoBehaviour
    {
        public CharAttributes attr;

        void Awake()
        {
            attr = GetComponent<CharAttributes>();
            if (attr == null)
            {
                attr = gameObject.AddComponent<CharAttributes>(); // may need some testing!
            }
        }
    }
}