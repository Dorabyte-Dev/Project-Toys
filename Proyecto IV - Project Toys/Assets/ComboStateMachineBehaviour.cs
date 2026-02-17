using System;
using UnityEngine;
using UnityEngine.Animations;

public class ComboStateMachineBehaviour : StateMachineBehaviour
{
    private Player _player;
    private event Action OnComboAttackExit;
    private event Action OnComboStateMachineExit;
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash, controller);
        _player = animator.GetComponent<Player>();
        OnComboAttackExit += _player.OnComboAttackEnded;
        OnComboStateMachineExit += _player.OnComboEnded;
        
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);
        OnComboStateMachineExit?.Invoke();
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(layerIndex)) return; // Evitamos lecturas raras durante los fundidos

        // Si la animación cruzó nuestro umbral y no hemos avisado aún...
        /*if (stateInfo.normalizedTime >= finishThreshold && !hasReported)
        {
            hasReported = true;
            OnComboAttackExit?.Invoke(); // ¡Llamamos al Player!
        }*/
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
