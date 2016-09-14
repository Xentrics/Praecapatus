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
    class EntityController : MonoBehaviour, Interactable
    {
        protected EntityMovement moveComp;
        protected AbilityManager abiCon;
        protected TestManager testManager;

        protected bool _bInfight = false;

        protected float _meleeRange = 10f;  // ?
        protected bool _inanimate = false;
        protected float _weight = 10f;      // ?

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

        public virtual void executeAbilityWith(EAbilities A, int version = 0, int minRP = 0)
        {
            testManager.testInstant(version, minRP, abiCon.getAbility(A), this, null);
        }

        /*********
         * GETTER AND SETTER
         ************************/

        public void toggleRunning()
        {
            moveComp.toggleRunning();
        }

        public bool running
        {
            get
            {
                return moveComp.running;
            }

            set
            {
                moveComp.running = value;
            }
        }

        public bool bInfight
        {
            get
            {
                return _bInfight;
            }

            set
            {
                _bInfight = value;
            }
        }

        public float weight
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool inanimate
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Vector3 position
        {
            get
            {
                return moveComp.position;
            }
        }

        public float MeleeRange
        {
            get
            {
                return _meleeRange;
            }
        }
    }
}
