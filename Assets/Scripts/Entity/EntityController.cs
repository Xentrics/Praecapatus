using Assets.Scripts.Abilities;
using Assets.Scripts.Items;
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
    public class EntityController : MonoBehaviour
    {
        protected AbilityManager    abiCon;         // all abilities available
        protected EntityInfo        entityInfo;     // basic attributes
        protected EntityMovement    moveComp;       // physical object in the world
        protected EntityObject      _praeObject;    // standard praecapatus object information. Weight, melee range, etc.
        [SerializeField] protected Inventory         _inventory;
        protected TestManager       DEB_testManager;// reference for debugging reasons. TODO: Should be performed by GameLogic later on

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
        {}


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
         * XML INTERFACE
         ************************/

        public System.Collections.Generic.List<AbilitySaveData> abiList
        {
            get { return abiCon.abiList; }
            set
            {
                if (value == null)
                    Debug.LogError("abiList cannot be set to NULL!");
                else
                    abiCon.abiList = value;
            }
        }

        public System.Collections.Generic.List<AttributeGroupSaveData> attrGrpList
        {
            get { return entityInfo.attrGrpList; }
            set
            {
                if (value == null)
                    Debug.LogError("attrGrpList cannot be set to NULL!");
                else
                    entityInfo.attrGrpList = value;
            }
        }

        public System.Collections.Generic.List<AttributeOtherSaveData> attrOtherList
        {
            get { return entityInfo.attrOtherList; }
            set
            {
                if (value == null)
                    Debug.LogError("attrOtherList cannot be set to NULL!");
                else
                    entityInfo.attrOtherList = value;
            }
        }

        public Inventory inventory
        {
            get { return _inventory; }
            set
            {
                if (value == null)
                    Debug.LogError("inventory cannot be set to NULL!");
                else
                    _inventory = value;
            }
        }

        /*********
         * GETTER AND SETTER
         ************************/

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

        public void drainMana(int by)
        {
            entityInfo.drainMaP(by);
        }

        public Vector3 lookDir
        {
            get
            {
                return moveComp.lookDir;
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

        public PraeObject praeObject
        {
            get
            {
                return _praeObject;
            }
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

        public void toggleRunning()
        {
            moveComp.toggleRunning();
        }
    }
}
