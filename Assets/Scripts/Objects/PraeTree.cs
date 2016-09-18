using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class PraeTree : PraeObject
    {
        public static readonly string[] treeDescriptions =
        {
            "Den Baum kenne ich nicht!",
            "Eine gut geformte Kastanie.",
            "Nur eine Buche.",
            "Nur eine Lärche.",
            "Nur eine Birke."
        };

        public int _descriptionId;

        void Start()
        {
            if (_descriptionId < 0 || _descriptionId >= treeDescriptions.Length)
                throw new ArgumentOutOfRangeException("_descriptionId out of bounds!");
            _description = treeDescriptions[_descriptionId];
        }

        public int descriptionId
        {
            set
            {
                if (_descriptionId < 0 || _descriptionId >= treeDescriptions.Length)
                    throw new ArgumentOutOfRangeException("_descriptionId out of bounds!");
                _descriptionId = value;
            }

            get
            {
                return _descriptionId;
            }
        }
    }
}
