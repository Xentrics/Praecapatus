using Assets.Scripts.Conversations;
using UnityEngine;

class ConTester : MonoBehaviour
{
    public TextAsset conAsset;

    void Awake()
    {
        Conversation[] con = Conversation.loadFromGraphml(conAsset);
    }
}