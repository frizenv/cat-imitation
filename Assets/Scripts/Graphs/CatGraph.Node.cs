using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CatImitation
{
    public partial class CatGraph : ScriptableObject
    {
        [System.Serializable]
        public class Node
        {
            [SerializeField] private Mood _mood;
            [SerializeField] private Action _action;
            [SerializeField] private int _id;
            [SerializeReference] private Node _transition;

            [HideInInspector] public Rect Size;
            [HideInInspector] public string Text;
            [HideInInspector] public string MethodName = "Empty";
            private static readonly Vector2 _windowSize = new Vector2(150, 100);

            public int ID => _id;
            public Node Transition
            {
                get
                {
                    Node node = _transition != null ? _transition: this;
                    return node;
                }
            }

            public Mood Mood => _mood;
            public Action Action => _action;

            public Node(int id, Vector2 position)
            {
                _id = id;
                Size = new Rect(position, _windowSize);
            }

            public void SetPosition(Vector2 newPosition)
            {
                Size = new Rect(newPosition.x, newPosition.y, _windowSize.x, _windowSize.y);
            }

            public void SetTransition(Node newTransition) => _transition = newTransition;
            public void SetMood(Mood mood) => _mood = mood;
            public void SetAction(Action action) => _action = action;

#if UNITY_EDITOR
            public GUIStyle ChooseStyle()
            {
                string style = "flow node 3 on";

                GUIStyle finalStyle = new GUIStyle(style)
                {
                    fixedHeight = Size.height,
                    fixedWidth = Size.width,
                    fontSize = 14,
                    contentOffset = new Vector2(0, -30)
                };
                return finalStyle;
            }
#endif
        }

        [System.Serializable]
        public class ActionWrapper
        {
            [SerializeField] private Action _action;
            [SerializeField, HideInInspector] private int _id;
            public List<Node> Nodes = new List<Node>();
            public Action Action => _action;
            public int ID => _id;

            public ActionWrapper(Action action, int id, IEnumerable<Node> nodes)
            {
                _action = action;
                _id = id;
                Nodes = nodes.ToList();
            }

            public void SetAction(Action newAction)
            {
                _action = newAction;
                Nodes.ForEach(node => node.SetAction(newAction));
            }

#if UNITY_EDITOR
            public GUIStyle ChooseStyle()
            {
                string style = "flow node 1 on";

                GUIStyle finalStyle = new GUIStyle(style)
                {
                    fixedHeight = 100,
                    fixedWidth = 100,
                    fontSize = 14,
                    contentOffset = new Vector2(0, -30)
                };
                return finalStyle;
            }
#endif
        }

        [System.Serializable]
        public class MoodWrapper
        {
            [SerializeField] private Mood _mood;
            [SerializeField, HideInInspector] private int _id;

            public event System.Action<MoodWrapper> OnMoodChanged = delegate { };
            public int ID => _id;
            public Mood Mood => _mood;

            public void SetMood(Mood mood)
            {
                _mood = mood;
                OnMoodChanged?.Invoke(this);
            }

            public MoodWrapper(int id)
            {
                _id = id;
            }

#if UNITY_EDITOR
            public GUIStyle ChooseStyle()
            {
                string style = "flow node 2 on";

                GUIStyle finalStyle = new GUIStyle(style)
                {
                    fixedHeight = 100,
                    fixedWidth = 150,
                    fontSize = 14,
                    contentOffset = new Vector2(0, -30)
                };
                return finalStyle;
            }
#endif
        }
    }
}