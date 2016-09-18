using Assets.Scripts.Abilities;
using Assets.Scripts.Managers;
using Assets.Scripts.Objects;
using UnityEngine;

namespace Assets.Scripts.Entity
{
    [RequireComponent(typeof(EntityMovement))]
    [RequireComponent(typeof(AbilityManager))]
    [RequireComponent(typeof(TestManager))]
    [RequireComponent(typeof(EntityObject))]
    [RequireComponent(typeof(EntityInfo))]
    class EntityController : MonoBehaviour
    {
        protected EntityInfo        entityInfo;
        protected EntityMovement    moveComp;       // physical object in the world
        protected AbilityManager    abiCon;         // all abilities available
        protected TestManager       DEB_testManager;    // reference for debugging reasons. TODO: Should be performed by GameLogic later on
        protected EntityObject      _praeObject;    // standard praecapatus object information. Weight, melee range, etc.

        protected bool  _bInfight   = false;

        protected bool  _inanimate  = false;
        protected float _weight     = 10f;      // ?

        protected virtual void Awake()
        {
            moveComp = GetComponent<EntityMovement>();
            abiCon = GetComponent<AbilityManager>();
            DEB_testManager = GetComponent<TestManager>();
            _praeObject = GetComponent<EntityObject>();
            entityInfo = GetComponent<EntityInfo>();
            praeObject.meleeRange = 10f;    // ?
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
            DEB_testManager.testInstant(version, minRP, abiCon.getAbility(A), this, null);
        }

        /*********
         * GETTER AND SETTER
         ************************/

        public void drainMana(int by)
        {
            entityInfo.drainMaP(by);
        }

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

        public Vector3 position
        {
            set
            {
                moveComp.position.Set(value.x, value.y, value.z); // use with caution...
            }

            get
            {
                return moveComp.position;
            }
        }

        public Vector3 lookDir
        {
            get
            {
                return moveComp.lookDir;
            }
        }

        public PraeObject praeObject
        {
            get
            {
                return _praeObject;
            }
        }
    }
}
