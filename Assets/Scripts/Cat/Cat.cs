using CatImitation.UI;
using TMPro;
using UnityEngine;

namespace CatImitation
{
    public class Cat : MonoBehaviour
    {
        [SerializeField] private Mood _currentMood;
        [SerializeField] private CatGraph _graph;
        [SerializeField] private MoodIndicatorUI _moodIndicator;
        [SerializeField] private ActionsUI _actions;
        [SerializeField] private TextMeshProUGUI _reactionText;
        [SerializeField] private CatController _catController;
        [SerializeField] private MovePoints _movePoints;

        private event System.Action _delayedAction = delegate { };
        public CatGraph Graph => _graph;

        private void Start()
        {
            SetGraph(_graph);
            SetCatController(_catController);
        }

        public void SetCatController(CatController catController)
        {
            if (catController == null) return;
            _catController = catController;
            catController.Initialize(this);
        }

        public void UseAction(Action action)
        {
            CatGraph.Node startNode = _graph.LocateNode(action, _currentMood);
            CatGraph.Node endNode = startNode.Transition;
            _delayedAction = () => DelayedAction(startNode, endNode);
            if (string.IsNullOrEmpty(startNode.MethodName) == false)
                _catController.Invoke(startNode.MethodName, 0f);
        }

        private void DelayedAction(CatGraph.Node startNode, CatGraph.Node endNode)
        {
            _currentMood = endNode.Mood;
            _moodIndicator.SetMood(endNode.Mood);
            SetReactionText(startNode.Text);
        }

        public void ContinueAction()
        {
            _delayedAction?.Invoke();
        }

        public void SetReactionText(string text) => _reactionText.SetText(text);

        public void SetGraph(CatGraph graph)
        {
            _graph = graph;
            _moodIndicator.Initialize(graph, _currentMood);
            _actions.Initialize(this);
        }

        public void RunAround()
        {
            _catController.Enqueue(_movePoints.RunAroundPoints);
            _catController.Enqueue(_movePoints.StartingPoint.position);
        }

        public void Eat()
        {

        }
    }
}