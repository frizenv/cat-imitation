using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CatImitation
{
    [CreateAssetMenu(menuName = "Scriptable Object/Cat graph")]
    public partial class CatGraph : ScriptableObject
    {
        [SerializeField, HideInInspector] private int _currentID;
        [HideInInspector] public List<ActionWrapper> Actions = new List<ActionWrapper>();
        [HideInInspector] public List<MoodWrapper> Moods = new List<MoodWrapper>();
        [HideInInspector] public Vector2 ScrollPosition;

        public IEnumerable<Node> Nodes => Actions.SelectMany(list => list.Nodes);

        public Node GetNode(int id) => Nodes.FirstOrDefault(node => node.ID == id);
        public ActionWrapper GetActionWrapper(int id) => Actions.FirstOrDefault(action => action.ID == id);
        public MoodWrapper GetMoodWrapper(int id) => Moods.FirstOrDefault(mood => mood.ID == id);

        private void OnEnable()
        {
            foreach (MoodWrapper wrapper in Moods)
                wrapper.OnMoodChanged += AssignMood;
        }

        private void OnDisable()
        {
            foreach (MoodWrapper wrapper in Moods)
                wrapper.OnMoodChanged -= AssignMood;
        }

        public Node LocateNode(Action action, Mood mood)
        {
            Node start = Nodes.First(node => node.Action == action && node.Mood == mood);
            return start;
        }

        public Node CreateNode()
        {
            Node newNode = new Node(_currentID, Vector2.zero);
            _currentID++;
            return newNode;
        }

        public void AddAction()
        {
            List<Node> newRowOfNodes = new List<Node>();
            foreach (MoodWrapper wrapper in Moods)
            {
                Node newNode = CreateNode();
                newNode.SetMood(wrapper.Mood);
                newRowOfNodes.Add(newNode);
            }
            Actions.Add(new ActionWrapper(null, _currentID, newRowOfNodes));
            _currentID++;
            SetPositions();
        }

        public void AddMood()
        {
            MoodWrapper newWrapper = new MoodWrapper(_currentID);
            Moods.Add(newWrapper);
            _currentID++;
            newWrapper.OnMoodChanged += AssignMood;
            foreach (ActionWrapper wrapper in Actions)
            {
                Node newNode = CreateNode();
                newNode.SetAction(wrapper.Action);
                wrapper.Nodes.Add(newNode);
            }
            SetPositions();
        }

        public void SetPositions()
        {
            for (int i = 0; i < Actions.Count; i++)
                for (int j = 0; j < Actions[i].Nodes.Count; j++)
                    Actions[i].Nodes[j].SetPosition(new Vector2(200, 200) + new Vector2(j * 200, i * 150));
        }

        public void RemoveAction(ActionWrapper wrapper)
        {
            Actions.Remove(wrapper);
            SetPositions();
        }

        public void RemoveMood(MoodWrapper wrapper)
        {
            int index = Moods.IndexOf(wrapper);
            foreach (ActionWrapper action in Actions)
                action.Nodes.RemoveAt(index);
            Moods.RemoveAt(index);
            SetPositions();
        }

        public void Reset()
        {
            Moods.Clear();
            Actions.Clear();
            _currentID = 0;
        }

        private void AssignMood(MoodWrapper wrapper)
        {
            int index = Moods.IndexOf(wrapper);
            foreach (ActionWrapper action in Actions)
                if (index >= 0)
                    action.Nodes[index].SetMood(wrapper.Mood);
        }

    }

}