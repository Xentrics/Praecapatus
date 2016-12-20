using Assets.Scripts.Items;
using Assets.Scripts.Quests;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class CentralInformationInterface : MonoBehaviour
    {
        [SerializeField] List<Quest> acceptedQuests;
        [SerializeField] HashSet<string> finishedQuests;
        [SerializeField] HashSet<PraeItem> questItems;
        [SerializeField] HashSet<KeyValuePair<string, int>> conOptionListener; // quest name -> option id

        void Start()
        {
            QuestLoader.LoadQuestStatesFromDisk(out acceptedQuests, out finishedQuests);
            // update all hashsets, listeners and so on
            foreach (Quest q in acceptedQuests)
            {
                GatherGoalsFromQuestOptions(new List<QuestOption>() { q.GetCurrentNode().GetMainGoal() });
                GatherGoalsFromQuestOptions(q.GetCurrentNode().GetOptions());
                GatherGoalsFromQuestOptions(q.GetCurrentNode().GetVoluntaries());
            }
        }


        void OnApplicationQuit()
        {
            QuestLoader.SaveQuestStatesToDisk(acceptedQuests, finishedQuests);
        }


        protected void GatherGoalsFromQuestOptions(List<QuestOption> options)
        {
            foreach (QuestOption q in options)
            {
                foreach (QuestGoal g in q.goals)
                {
                    switch(g.questType)
                    {
                        case EQuestType.FIND:
                            // TODO:
                            break;
                        case EQuestType.GATHER:
                            // TODO:
                            break;
                        case EQuestType.DELIVER:
                            // TODO:
                            break;
                        case EQuestType.CON_NODE:
                            // TODO:
                            break;
                        default:
                            throw new System.Exception("CII: goal of new but undefined type detected! : " + g.goalType.Name);
                    }
                }
            }
        }
    }
}
