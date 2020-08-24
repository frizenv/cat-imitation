using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CatImitation
{
    public class CatController : MonoBehaviour
    {
        [SerializeField] private MovePoints _points;
        private NavMeshAgent _agent;
        private Animator _animator;
        private Cat _cat;
        private Queue<Vector3> _movePoints = new Queue<Vector3>();
        private bool _readyToMove = true;
        private bool _locked;
        private Vector3 _destination;
        private event System.Action _interruptReaction = delegate { };

        private readonly int _hashSpeed = Animator.StringToHash("Speed");
        private readonly int _hashEat = Animator.StringToHash("Eat");
        private readonly int _hashJump = Animator.StringToHash("Jump");
        private readonly int _hashSound = Animator.StringToHash("Sound");
        private const float k_accuracy = 2f;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _animator.SetFloat(_hashSpeed, (_agent.velocity / _agent.speed).magnitude);
            if (_readyToMove && _movePoints.Count != 0)
                MoveTo(_movePoints.Dequeue());
            if (_readyToMove == false && Vector3.SqrMagnitude(transform.position - _destination) < k_accuracy * k_accuracy)
            {
                _readyToMove = true;
                _locked = false;
            }
        }

        public void Initialize(Cat cat)
        {
            _cat = cat;
        }

        public void Enqueue(Vector3 movePoint)
        {
            _movePoints.Enqueue(movePoint);
        }

        public void Enqueue(IEnumerable<Transform> movePoints)
        {
            foreach (Transform point in movePoints)
                Enqueue(point.position);
        }

        public void MoveTo(Vector3 destination)
        {
            _destination = destination;
            _agent.SetDestination(destination);
            _readyToMove = false;
            _locked = true;
        }

        public void RunAround() 
        {
            if (_locked)
            {
                _interruptReaction?.Invoke();
                return;
            }
            Enqueue(_points.RunAroundPoints);
            Enqueue(_points.StartingPoint.position);
            _cat.ContinueAction();
        }

        public void RunAway()
        {
            if (_locked)
            {
                _interruptReaction?.Invoke();
                return;
            }
            Enqueue(_points.RunawayPoint.position);
            Enqueue(_points.StartingPoint.position);
            _cat.ContinueAction();
        }

        public void Eat()
        {
            if (_locked)
            {
                _interruptReaction?.Invoke();
                return;
            }
            _animator.Play(_hashEat);
            _locked = true;
            _cat.ContinueAction();
        }

        public void EatCautiously()
        {
            if (_locked)
            {
                _interruptReaction?.Invoke();
                return;
            }
            _animator.Play(_hashEat);
            _interruptReaction = () => { _animator.Play(_hashJump); _cat.SetReactionText("царапает"); };
            _locked = true;
            _cat.ContinueAction();
        }

        public void Jump()
        {
            if (_locked)
            {
                _interruptReaction?.Invoke();
                return;
            }
            _animator.Play(_hashJump);
            _locked = true;
            _cat.ContinueAction();
        }

        public void Sound()
        {
            if (_locked)
            {
                _interruptReaction?.Invoke();
                return;
            }
            _animator.Play(_hashSound);
            _locked = true;
            _cat.ContinueAction();
        }

        public void Empty()
        {
            if (_locked)
            {
                _interruptReaction?.Invoke();
                return;
            }
            _cat.ContinueAction();
        }

        public void ResetState()
        {
            _interruptReaction = delegate { };
            _locked = false;
        }
    }
}