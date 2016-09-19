﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static int score;


        Text text;


        void Awake()
        {
            text = GetComponent<Text>();
            score = 0;
        }


        void Update()
        {
            text.text = "Score: " + score;
        }
    }
}
