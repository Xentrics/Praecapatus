using Assets.Scripts.Abilities;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Entity;

namespace Assets.Scripts.Managers
{
    [System.Serializable]
    public class AbilityManager : MonoBehaviour
    {
        public static readonly int DEFAULT_ABILITY_LEVEL = 0;
        public static readonly int UNLEARNED = 0;

        PlayerController playerC;
        Dictionary<EAbilities, AbstractAbility> abilities;

        public void Awake()
        {
            if (playerC == null)
                playerC = GetComponent<PlayerController>();

            abilities = new Dictionary<EAbilities, AbstractAbility>();
            abilities.Add(EAbilities.null_, new NullAbility());
            abilities.Add(EAbilities.test, new AbilityTest());
            abilities.Add(EAbilities.Astrahlbelebung, new AbilityAstralbelebung());
            // TODO: add abilities here
        }

        // XML saving object
        public List<AbilitySaveData> abiList
        {
            get
            {
                List<AbilitySaveData> valArr = new List<AbilitySaveData>(Constants.NUM_ABILITIES+1);
                int i = 0;
                foreach (var ak in abilities)
                {
                    valArr.Add(new AbilitySaveData());
                    valArr[i].abi  = ak.Key;
                    valArr[i].fw   = ak.Value.fw;
                    valArr[i].mode = ak.Value.usageMode;
                    ++i;
                }
                
                return valArr;
            }

            set
            {
                foreach (AbilitySaveData a in value)
                {
                    abilities[a.abi].fw = a.fw;
                    abilities[a.abi].usageMode = a.mode;
                }
            }
        }

        public AbstractAbility getAbility(EAbilities A)
        {
            return abilities[A];
        }
    }
}
