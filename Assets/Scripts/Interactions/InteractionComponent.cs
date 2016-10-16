using Assets.Scripts.Entity;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interactions
{
    /**
     * stores all kinds of conversations and other interactions
     * - it should also compensate for missing attributes
     */
    public class InteractionComponent : MonoBehaviour
    {
        public EntityController _ec;
        public Inventory _inv;
        List<TextAsset> conversations;

        void Awake()
        {
            Debug.Log("inter comp");
            _ec = GetComponent<EntityController>();
            _inv = (_ec) ? _ec.inventory : null;

            //Debug.Log("inter: " + _ec.praeObject.name);
            _inv.AddItem(new PraeItem("hala", 1.0f, new Currency(1, 2, 3), 1, 2, null));
        }

        public void addInteractionOption(object inter)
        {
            throw new NotImplementedException();
        }

        public void StartConversation(InteractionComponent target)
        {
            TextAsset conAsset = Resources.Load<TextAsset>("Conversations/test_g1"); // just for testing!
            Conversation[] con = Conversation.loadFromGraphml(conAsset);
            Debug.Assert(target != null);
            Constants.interactionManager.StartInteraction(this, target, con[0]);
        }

        /**
         * GETTER AND SETTER
         **********************/

        public bool hasController() { return _ec != null; }
        public bool hasInventory() { return _inv != null; }

        public Inventory inventory
        {
            get
            {
                return (_inv != null) ? _inv : new Inventory(); // return empty inventory
            }
        }

        public EntityController ec
        {
            get { return _ec; }
        }
    }
}
