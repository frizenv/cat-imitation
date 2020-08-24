using UnityEngine;

namespace CatImitation.StateMachines
{
    public class ResetState : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<CatController>().ResetState();
        }
    }
}