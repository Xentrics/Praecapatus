using System;
using UnityEngine;

namespace Assets.Scripts.Conversations
{
    public class InteractionComponent : MonoBehaviour
    {
        public void addInteractionOption(object inter)
        {
            throw new NotImplementedException();
        }

        public void StartConversation()
        {
            TextAsset conAsset = Resources.Load<TextAsset>("Conversations/test_g1"); // just for testing!
            Conversation[] con = Conversation.loadFromGraphml(conAsset);
            Constants.interactionManager.StartInteraction(con[0]);
        }
    }
}
