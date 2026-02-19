using System;
using UnityEngine;
using UnityEngine.Animations;

public class ComboStateMachineBehaviour : StateMachineBehaviour
{
    private Player _player;
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash, controller);
        if (_player == null) _player = animator.GetComponent<Player>();
        
        Debug.LogWarning("Entered Combo State Machine");
        
        
    }
    
    

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);
        
        if (_player == null) _player = animator.GetComponent<Player>();
        
        animator.ResetTrigger("LightTrigger");
        animator.ResetTrigger("HeavyTrigger");
        _player.OnComboEnded();
        
        Debug.LogWarning("Exit Combo State Machine");
        
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
       
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        
    //}

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
