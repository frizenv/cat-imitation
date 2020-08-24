using UnityEngine;

namespace CatImitation
{
    [CreateAssetMenu(menuName = "Scriptable Object/Action")]
    public class Action : ScriptableObject
    {
        [SerializeField] private string _actionName;

        public string ActionName => _actionName;
    }
}