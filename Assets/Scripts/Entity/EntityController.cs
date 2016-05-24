using Assets.Scripts.Abilities;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Assets.Scripts.Entity
{
    [RequireComponent(typeof(EntityMovement))]
    [RequireComponent(typeof(AbilityManager))]
    [RequireComponent(typeof(TestManager))]
    class EntityController : MonoBehaviour
    {
        protected EntityMovement moveComp;
        protected AbilityManager abiCon;
        protected TestManager testManager;

        void Awake()
        {
            moveComp = GetComponent<EntityMovement>();
            abiCon = GetComponent<AbilityManager>();
            testManager = GetComponent<TestManager>();
        }

        /*
         * handle correct key input across multiple scripts & mechanics
         */
        void LateUpdate()
        {
        }

        public virtual Vector3 getPosition()
        {
            return moveComp.getPosition();
        }

        public virtual float getMeleeRange()
        {
            return 10; // 1 meter?
        }

        public virtual void executeAbilityWith(EAbilities A, int minRP = 0)
        {
            testManager.testInstant(minRP, abiCon.getAbility(A), this, null);
        }
    }
}
