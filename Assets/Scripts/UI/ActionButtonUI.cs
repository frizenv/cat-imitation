using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CatImitation.UI
{
    [RequireComponent(typeof(Button))]
    public class ActionButtonUI : MonoBehaviour
    {
        private Action _action;

        public void SetAction(Action action, Cat cat)
        {
            _action = action;
            GetComponent<Button>().onClick.AddListener(() => cat.UseAction(_action));
            GetComponentInChildren<TextMeshProUGUI>().SetText(action.ActionName);
        }

    }
}