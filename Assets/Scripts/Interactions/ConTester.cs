using UnityEngine;

namespace Assets.Scripts.Interactions
{
    class ConTester : MonoBehaviour
    {
        public TextAsset conAsset;

        void Awake()
        {
        }

        void Start()
        {
            /*
            Conversation[] con = Conversation.loadFromGraphml(conAsset);
            Constants.interactionManager.StartInteraction(con[0]);
            */
            throw new System.Exception("This function is no longer valid!");
        }
    }
}