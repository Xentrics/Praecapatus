using UnityEngine;

namespace Assets.Scripts.Interactions
{
    class ConTester : MonoBehaviour
    {
        public TextAsset conAsset;
        public XGMLGraph graph;

        void Awake()
        {
        }

        void Start()
        {
            graph = Conversation.loadFromXGML(conAsset);
        }
    }
}