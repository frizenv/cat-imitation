using UnityEngine;

namespace CatImitation
{
    [CreateAssetMenu(menuName = "Scriptable Object/Mood")]
    public class Mood : ScriptableObject
    {
        [SerializeField] private string _name;

        public string Name => _name;
    }
}