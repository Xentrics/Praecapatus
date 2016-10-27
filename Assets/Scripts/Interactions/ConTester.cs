using UnityEngine;

namespace Assets.Scripts.Interactions
{
    class ConTester : MonoBehaviour
    {
        public TextAsset conAsset;
        //public C graph;

        void Awake()
        {
        }

        void Start()
        {
            Conversation[] cons = Conversation.loadFromGephiGraphML(conAsset);
        }
    }
}