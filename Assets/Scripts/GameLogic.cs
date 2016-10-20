using Assets.Scripts.Interactions;
using Assets.Scripts.Entity;
using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /**
     * - handles dialogs
     * - handles entities
     * - handles world generation
     * - handles transitions: exploration | fighting | ...
     */
    [RequireComponent(typeof(SanityChecker))]
    public class GameLogic : MonoBehaviour
    {
        struct timedInteraction
        {
            public InteractionComponent comp;
            public DateTime endTime;

            public timedInteraction(InteractionComponent i, DateTime endTime)
            {
                comp = i;
                this.endTime = endTime;
            }
        };

        GameObject _mainChar;
        EntityController _pc;
        private Quaternion _worldViewRotation = Quaternion.Euler(0, 0, 0); // THIS REFERENCE SHOULD NEVER CHANGE
        private Quaternion worldViewBackupReference; // Bug stuff. Leads to critical fail if it becomes a different reference that '_worldViewRotation'

        // minor world rotation integration
        private float worldViewRotationSpeed = 0.3f; // 1 second?
        private bool isRotating = false;
        private Quaternion rotationDestination;
        List<GameObject> sceneObjects = null;

        HashSet<timedInteraction> timedInteractions = new HashSet<timedInteraction>(); // will contain tempoary interactioncomps. Includes those from 'Astralbelebung'

        public bool shouldSaveData = true;

        public void Awake()
        {
            worldViewBackupReference = _worldViewRotation; // remember reference

            // TODO: spawn world here??
            Vector3[] treepos = Useful.RandomPointsInRing(1000, 25, 100);
            foreach (Vector3 v in treepos)
                TreeFactory.makeRandomTreeAtPos(v, true);

            Constants.gameLogic = this; // set up central reference
        }

        public void Start()
        {
            _mainChar = GameObject.FindGameObjectWithTag("MainCharacter");
            _pc = _mainChar.GetComponent<EntityController>();

            if (!_mainChar || !_pc)
                //TODO: LOAD CHARACTER FORM PRESET!
                throw new NullReferenceException("GameManager could not find mainCharacter or his attached controller!!!!");
        }


        /**
         * func: add i to the pool of temporary interactors. if DateTime.Now > endTime, i will be destroyed
         * @i:       component to be destroyed when DateTime.Now > endTime
         * @endTime: terminal condition
         */
        public void AddTimedInteraction(InteractionComponent i, DateTime endTime)
        {
            Debug.Assert(endTime > DateTime.Now, "Do not add a timed interaction with endTime < startTime!");
            timedInteractions.Add(new timedInteraction(i, endTime));
        }

        /**
         * handles:
         * - map view rotation
         * - timed destruction of timed interactions
         */
        public void LateUpdate()
        {
            // this reference must never change!
            //Debug.Assert(worldViewBackupReference == _worldViewRotation, "CRITICAL ERROR: '_worldViewRotation' OBJECT CHANGED REFERENCE!");

            if (isRotating)
            {
                print(worldViewRotation);
                worldViewRotation = Quaternion.Lerp(_worldViewRotation, rotationDestination, worldViewRotationSpeed); // remember: we overloaded the setter!
                if (_worldViewRotation == rotationDestination)
                {
                    isRotating = false;
                    Debug.Log("GameManager finished rotating.");
                    //sceneObjects = GetAllGameObjectsInScene();
                }

                // update rotation of all game objects in the scene
                // UI elements are excluded
                foreach (var o in sceneObjects)
                {
                    Rigidbody rb = o.GetComponent<Rigidbody>();
                    if (rb == null)
                        o.transform.rotation = worldViewRotation;
                    else
                        rb.rotation = worldViewRotation; // it is better to let the rigidbody handle rotation, if present
                }
                
            }

            // take care of temporary interactions
            DateTime now = DateTime.Now;
            timedInteractions.RemoveWhere(delegate (timedInteraction x) {
                if (now >= x.endTime)
                {
                    Destroy(x.comp); // remove component
                    return true;
                }
                else
                    return false;
            });
        }

        

        /**
         * @leftwise: TRUE -> turn 90° leftwise. Else -> turn 90° rightwise
         */
        public void rotateWorldView(bool leftwise)
        {
            if (!isRotating)
            {
                rotationDestination = _worldViewRotation * Quaternion.Euler(0, (leftwise) ? 90 : -90, 0);
                isRotating = true;
                sceneObjects = GetAllGameObjectsInScene();
                Debug.Log("GameManager starts with map rotation...");
            }
            else
                Debug.Log("GameManager is already rotating. 'rotateWorldView' ignored.");
        }

        /**
         * func: 
         */
        List<GameObject> GetAllGameObjectsInScene()
        {
            int ignoreLayer = 5;
            List<GameObject> objects = new List<GameObject>();
            foreach (var o in FindObjectsOfType<GameObject>())
            {
                if (o.layer != ignoreLayer)
                    objects.Add(o);
            }

            return objects;
        }

        

        /**
         * GETTER AND SETTER
         ***************************/
        
        public EntityController pc
        {
            get { return _pc; }
        }

        public GameObject mainChar
        {
            get { return _mainChar; }
        }

        public Quaternion worldViewRotation
        {
            private set
            {
                _worldViewRotation.x = value.x;
                _worldViewRotation.y = value.y;
                _worldViewRotation.z = value.z;
                _worldViewRotation.w = value.w; // DO NOT BRAKE ANY REFERENCES!
            }

            get
            {
                return _worldViewRotation;
            }
        }
    }
}
