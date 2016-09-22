using Assets.Scripts;
using Assets.Scripts.Conversations;
using UnityEngine;

class ConTester : MonoBehaviour
{
    public TextAsset conAsset;

    void Awake()
    {
    }

    void Start()
    {
        Conversation[] con = Conversation.loadFromGraphml(conAsset);
        Constants.interactionManager.StartInteraction(con[0]);
    }
}