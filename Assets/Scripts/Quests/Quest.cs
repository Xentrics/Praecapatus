using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Quests
{
    [Serializable]
    public class Quest
    {
        public string title;
        public int id;
        QuestNode startQuestNode;
        [SerializeField] QuestNode currentQuestNode;
        [SerializeField] QuestNode[] questNodes;

        public Quest(QuestNode start, QuestNode[] nodes, string title)
        {
            if (start == null)
                throw new NullReferenceException("start nodes for a quest must not be null!");
            startQuestNode = start;
            currentQuestNode = start;
            this.title = title;
            Debug.Log("Quest loaded: " + title);
            questNodes = nodes;
        }

        public string currentNodeId
        {
            get { return currentQuestNode.xmlID;}
        }

        private QuestNode GetNode(string xmlId)
        {
            foreach (QuestNode n in questNodes)
                if (n.xmlID == xmlId)
                    return n;
            return null;
        }

        public void SetToNode(string nodeId)
        {
            if (nodeId == null || nodeId == "")
                throw new NullReferenceException("Cannot switch to node with empty id!");

            QuestNode n = GetNode(nodeId);
            if (n != null)
                currentQuestNode = n;
            else
                Debug.Log("Did not find node with id " + nodeId + " for quest " + title);
        }
    }

    /**
     * - basically a collection of goals which may return a reword when reached
     * - represented by nodes in graphml
     */
    [Serializable]
    public class QuestNode
    {
        public string xmlID;
        public string label;
        public int alignment;
        List<IQuestGoal> goals = new List<IQuestGoal>();
        List<QuestOption> options = new List<QuestOption>();   // one of these must be fulfilled completely to solve this quest
        List<QuestOption> voluntaries = new List<QuestOption>(); // these are optional, but may result in bigger rewards

        public void AddGoal(IQuestGoal goal)
        {
            if (goal == null)
                throw new NullReferenceException("A quest goal cannot be added: reference is NULL!");

            goals.Add(goal);
        }

        public void AddGoals(List<IQuestGoal> goals)
        {
            if (goals == null)
                throw new NullReferenceException("A quest goal cannot be added: reference is NULL!");

            goals.AddRange(goals);
        }

        public void AddOption(QuestOption o, bool voluntary = false)
        {
            if (o == null)
                throw new NullReferenceException("Cannot add quest option: input is null!");
            if (voluntary)
                voluntaries.Add(o);
            else
                options.Add(o);
        }
    }

    /**
     * - additional goals and a reference to the next quest node
     * - represented by edges in the graph
     */
    public class QuestOption
    {
        public QuestNode nextNode;
        public string label;
        public int alignment;
        List<IQuestGoal> goals;

        public void AddGoal(IQuestGoal goal)
        {
            if (goal == null)
                throw new NullReferenceException("A quest goal cannot be added: reference is NULL!");

            goals.Add(goal);
        }

        public void AddGoals(List<IQuestGoal> goals)
        {
            if (goals == null)
                throw new NullReferenceException("A quest goal cannot be added: reference is NULL!");

            goals.AddRange(goals);
        }
    }

    public interface IQuestReward
    {
        object reward { get; }
    }

    public class QuestReward<T> : IQuestReward
    {
        T _reward;
        Type _rewardType;

        public QuestReward(T rew)
        {
            _reward = rew;
            _rewardType = typeof(T);
        }

        public T reward
        {
            get { return _reward; }
        }

        object IQuestReward.reward
        {
            get { return _reward; }
        }

        public Type rewardType
        {
            get { return _rewardType; }
        }
    }

    public interface IQuestGoal
    {
        object goal { get; }
    }

    public class QuestGoal<Data> : IQuestGoal
    {
        string _desc;
        EQuestType _questType;
        Data _goal;
        Type _goalType;
        public bool bFinished;

        public QuestGoal(string desc, EQuestType questType, Data goal)
        {
            _desc = desc;
            _questType = questType;
            _goal = goal;
            _goalType = typeof(Data);
        }

        public string description
        {
            get { return _desc; }
        }

        public EQuestType questType
        {
            get { return _questType; }
        }

        public Data goal
        {
            get { return _goal; }
        }

        object IQuestGoal.goal
        {
            get { return _goal; }
        }

        public Type goalType
        {
            get { return _goalType; }
        }
    }

    public enum EQuestType
    {
        CON_NODE,
        DELIVER,
        FIND,
        GATHER,
    }
}
