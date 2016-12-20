using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Quests
{
    [Serializable]
    /**
     * - A quest is in essenace a graph of nodes (main goals + rewards) and edges (decisional and optional goals)
     * - an optional goal is implemented by an edge (QuestOption) makes as 'side'
     *  - the node succeeding such a goal will add to the full reward ONLY if the main quest is fulfilled
     */
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

        public QuestNode GetCurrentNode()
        {
            return currentQuestNode;
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
     * - basically a collection of goals which may return a reward when reached
     * - represented by nodes in graphml
     */
    [Serializable]
    public class QuestNode
    {
        public string xmlID;
        QuestOption main_goal = new QuestOption();
        List<QuestOption> options = new List<QuestOption>();   // if none empty, one of these must be fulfilled completely to solve this quest
        List<QuestOption> voluntaries = new List<QuestOption>(); // these are optional, but may result in bigger rewards

        public void AddGoal(QuestGoal goal)
        {
            if (goal == null)
                throw new NullReferenceException("A quest goal cannot be added: reference is NULL!");

            main_goal.AddGoal(goal);
        }

        public void AddGoals(List<QuestGoal> goals)
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

        //TODO: restrict data access
        public QuestOption GetMainGoal()
        {
            return main_goal;
        }

        public List<QuestOption> GetOptions()
        {
            return options;
        }

        public List<QuestOption> GetVoluntaries()
        {
            return voluntaries;
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
        public List<QuestGoal> goals;

        public void AddGoal(QuestGoal goal)
        {
            if (goal == null)
                throw new NullReferenceException("A quest goal cannot be added: reference is NULL!");

            goals.Add(goal);
        }

        public void AddGoals(List<QuestGoal> goals)
        {
            if (goals == null)
                throw new NullReferenceException("A quest goal cannot be added: reference is NULL!");

            goals.AddRange(goals);
        }
    }


    /**
     * Generic class keeping any kind of reward definable as a class
     */
    public class QuestReward<T>
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

        public Type rewardType
        {
            get { return _rewardType; }
        }
    }

    public abstract class QuestGoal
    {
        protected string _desc;
        protected EQuestType _questType;
        protected Type _goalType;
        public bool bFinished;

        public QuestGoal(string desc, EQuestType questType)
        {
            _desc = desc;
            _questType = questType;
            _goalType = typeof(object);
        }

        public string description
        {
            get { return _desc; }
        }

        public EQuestType questType
        {
            get { return _questType; }
        }

        public Type goalType
        {
            get { return _goalType; }
        }
    }

    /**
     * generic class to instanciate quest goals
     * also remembers the type used for 'Data'
     */
    public class QuestGoal<Data> : QuestGoal
    {
        Data _goal;

        public QuestGoal(string desc, EQuestType questType, Data goal) : base(desc, questType)
        {
            _goal = goal;
            _goalType = typeof(Data);
        }

        public Data goal
        {
            get { return _goal; }
        }
    }

    /**
     * - data container for conversation node goals
     * - con_node
     */
    public class ConNodeGoalData
    {
        public string conName;
        public string nodeId;

        public ConNodeGoalData(string name, string node)
        {
            conName = name;
            nodeId = node;
        }
    }

    /**
     * - data container for any goal that requires items
     * - find, gather
     */
    public class ItemGoalData
    {
        public Items.PraeItem item;
        public int amount;

        public ItemGoalData(Items.PraeItem item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    /**
     * - data container for any goal that requires items
     * - deliver
     */
    public struct ItemDeliverAllGoalData
    {
        public int charId;

        public ItemDeliverAllGoalData(int charId)
        {
            this.charId = charId;
        }
    }

    /**
     * - data container for any goal that requires items
     * - deliver
     */
    public class ItemDeliverGoalData
    {
        public List<Items.PraeItem> items;
        public List<int> amounts;
        public int charId;

        public ItemDeliverGoalData(int charId)
        {
            items = new List<Items.PraeItem>();
            amounts = new List<int>();
            this.charId = charId;
        }
    }

    /**
     * makes the transition from stored quest to actual game events easier
     */
    public enum EQuestType
    {
        CON_NODE,
        DELIVER,
        FIND,
        GATHER,
    }
}
