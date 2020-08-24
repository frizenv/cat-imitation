using System.Collections.Generic;
using UnityEngine;

namespace CatImitation
{
    public class MovePoints : MonoBehaviour
    {
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private List<Transform> _runAroundPoints;
        [SerializeField] private Transform _runAwayPoint;

        public Transform StartingPoint => _startingPoint;
        public Transform RunawayPoint => _runAwayPoint;
        public IReadOnlyList<Transform> RunAroundPoints => _runAroundPoints.AsReadOnly();
    }
}