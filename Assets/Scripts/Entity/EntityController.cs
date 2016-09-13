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

        protected virtual void Awake()
        {
            moveComp = GetComponent<EntityMovement>();
            abiCon = GetComponent<AbilityManager>();
            testManager = GetComponent<TestManager>();
        }

        protected virtual void Start()
        {

        }

        /*
         * handle correct key input across multiple scripts & mechanics
         */
        protected virtual void LateUpdate()
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

        public void toggleRunning()
        {
            moveComp.toggleRunning();
        }

        public void setIsRunning(bool b)
        {
            moveComp.setIsRunning(b);
        }

        public virtual void executeAbilityWith(EAbilities A, int version = 0, int minRP = 0)
        {
            testManager.testInstant(version, minRP, abiCon.getAbility(A), this, null);
        }
    }
}
