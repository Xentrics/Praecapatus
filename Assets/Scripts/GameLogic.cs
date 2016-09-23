using Assets.Scripts.Conversations;
using Assets.Scripts.Entity;
using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        GameObject mainChar;
        PlayerController pc;
        private Quaternion _worldViewRotation = Quaternion.Euler(0, 0, 0); // THIS REFERENCE SHOULD NEVER CHANGE
        private Quaternion worldViewBackupReference; // Bug stuff. Leads to critical fail if it becomes a different reference that '_worldViewRotation'

        // minor world rotation integration
        private float worldViewRotationSpeed = 1f; // 1 second?
        private bool isRotating = false;
        private Quaternion rotationDestination;

        HashSet<timedInteraction> timedInteractions = new HashSet<timedInteraction>(); // will contain tempoary interactioncomps. Includes those from 'Astralbelebung'


        public void Awake()
        {
            worldViewBackupReference = _worldViewRotation; // remember reference

            // spawn world here??
            Vector3[] treepos = PotentiallyUsefulStuff.RandomPointsInRing(1000, 25, 100);
            foreach (Vector3 v in treepos)
                TreeFactory.makeRandomTreeAtPos(v, true);

            Constants.gameLogic = this; // set up central reference
        }

        public void Start()
        {
            mainChar = GameObject.FindGameObjectWithTag("MainCharacter");
            pc = mainChar.GetComponent<PlayerController>();

            if (!mainChar || !pc)
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
            // since this one variable is absolutely crucial and can brake up everything,
            // I even go as far as to ensure this constraint
            Debug.Assert(worldViewBackupReference == _worldViewRotation, "CRITICAL ERROR: '_worldViewRotation' OBJECT CHANGED REFERENCE!");

            if (isRotating)
            {
                worldViewRotation = Quaternion.Lerp(_worldViewRotation, rotationDestination, worldViewRotationSpeed); // remember: we overloaded the setter!
                if (_worldViewRotation.Equals(rotationDestination))
                {
                    isRotating = false;
                    Debug.Log("GameManager finished rotating.");
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
         * SYNCHRONISED! NEVER CALL OUTSIDE!
         * @leftwise: TRUE -> turn 90° leftwise. Else -> turn 90° rightwise
         */
        private void rotateWorldView(bool leftwise)
        {
            if (!isRotating)
            {
                rotationDestination = _worldViewRotation * Quaternion.Euler(0, (leftwise) ? 90 : -90, 0);
                isRotating = true;
                Debug.Log("GameManager starts with map rotation...");
            }
            else
                Debug.Log("GameManager is already rotating. 'rotateWorldView' ignored.");
        }


        public Quaternion worldViewRotation
        {
            private set
            {
                _worldViewRotation.Set(value.x, value.y, value.z, value.w); // DO NOT BRAKE ANY REFERENCES!!!!!!!!!
            }

            get
            {
                return _worldViewRotation;
            }
        }
    }
}
