using System;
using UnityEngine;

namespace Assets.Scripts
{
    /**
     * These are basically all the basic properties of any PraeObject object
     * in the Praecapatus world
     */
    public class PraeObject : MonoBehaviour
    {
        public string[] descriptions = {
            "Nothing interesting."
        };

        [SerializeField] protected float _weight         = 0f;    // metrical, kg
        [SerializeField] protected bool  _inanimate      = true;  // FALSE: the object is not a living being with any regard
        [SerializeField] protected float _meleeRange     = 0f;
        [SerializeField] protected string _description;           // is shown when the object is not interactable for any reason
        [SerializeField] protected InteractionComponent interComp = null;
        [SerializeField] protected bool disableInteraction = false; // FALSE -> prevent interaction even if interComp is set

        void Awake()
        {
            if (_weight < 0 || _meleeRange < 0)
                throw new ArgumentException("Floats with illegal, negative values found!");
            _description = descriptions[0];
        }


        public InteractionComponent AddInteractionComponent()
        {
            if (interComp)
                throw new ArgumentException("Object already has interaction component!");

            interComp = gameObject.AddComponent<InteractionComponent>();
            return interComp;
        }

        /****
         * GETTER AND SETTER
         *******************/
        
        public float weight
        {
            get
            {
                return _weight;
            }

            set
            {
                if (value < 0)
                    throw new ArgumentException("Weights cannot be negative!");
                _weight = value;
            }
        }

        public bool inanimate
        {
            get
            {
                return _inanimate;
            }

            set
            {
                _inanimate = value;
            }
        }

        public Vector3 position
        {
            get
            {
                return transform.position;
            }
        }

        public float meleeRange
        {
            set
            {
                if (value < 0)
                    throw new ArgumentException("MeleeRange cannot be negative!");
                _meleeRange = value;
            }

            get
            {
                return _meleeRange;
            }
        }

        public string description
        {
            get
            {
                return _description;
            }
        }

        public bool tryInteract(PraeObject caller)
        {
            //TODO: handle InteractionComponent stuff
            return !disableInteraction && hasInteraction();
        }

        public bool hasInteraction()
        {
            return interComp != null;
        }
    }
}
