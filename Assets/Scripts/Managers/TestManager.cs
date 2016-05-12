using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers
{
    class TestManager : MonoBehaviour
    {
        // the inner classes will only be used here. They should handle variable packs easier
        struct instantTest
        {

        };

        struct prolongedTest
        {

        };

        // variables
        instantTest intTest;
        prolongedTest proTest;

        public void Awake()
        {

        }

        public void initiateNewTest(ETestMode mode)
        {
            switch (mode)
            {
                case ETestMode.instant:
                    break;
                case ETestMode.prolonged:
                    break;
                case ETestMode.custom:
                    break;
            }
        }

        private bool hasTestFinished()
        {
            throw new NotImplementedException();
        }

        private void useSuccessfulAbility(int by)
        {
            throw new NotImplementedException();
        }

        private void useFailedAbility(int by)
        {
            throw new NotImplementedException();
        }
    }
}
