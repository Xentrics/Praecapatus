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
        EntityController _ec;
        Inventory _inv;
        Shop _shop = null;      // the shop should go lost if the component is lost. Can be changed later on
        List<TextAsset> conversations;

        void Awake()
        {
            _ec = GetComponent<EntityController>();
            _inv = (_ec) ? _ec.inventory : null;
            _shop = GetComponent<Shop>();

            if (_shop != null)
                _ec.shopID = _shop.shopID;
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

        public Shop shop
        {
            get
            {
                if (_shop != null)
                    return _shop;
                else
                {
                    if (_inv == null)
                    {
                        _shop = gameObject.AddComponent<Shop>();
                        return _shop;
                    }
                    else
                    {
                        _shop = gameObject.AddComponent<Shop>();
                        _shop.Set(_inv);
                        return _shop;
                    }
                }
            }
        }

        public EntityController ec
        {
            get { return _ec; }
        }
    }
}
