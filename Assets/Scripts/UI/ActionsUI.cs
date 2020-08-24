using System.Collections.Generic;
using UnityEngine;

namespace CatImitation.UI
{
    public class ActionsUI : MonoBehaviour
    {
        [SerializeField] private ActionButtonUI _buttonPrefab;

        private readonly List<ActionButtonUI> _buttons = new List<ActionButtonUI>();

        public void Initialize(Cat cat)
        {
            for (int i = _buttons.Count- 1; i >= 0; i--)
                Destroy(_buttons[i].gameObject);
            _buttons.Clear();
            foreach (CatGraph.ActionWrapper wrapper in cat.Graph.Actions)
            {
                ActionButtonUI newButton = Instantiate(_buttonPrefab, this.transform);
                newButton.SetAction(wrapper.Action, cat);
                _buttons.Add(newButton);
            }
        }
    }
}