using UnityEngine;

public class Player_ExecutionState : PlayerState
{
    public Player_ExecutionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    private float executionDuration = 1f; //La idea es que se cambie luego cuando esté la animación de ejecución por un evento de animación

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered Player_ExecutionState");
        stateTimer = executionDuration;  //La idea es que se cambie luego cuando esté la animación de ejecución por un evento de animación
        
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer <= 0f)   //La idea es que se cambie luego cuando esté la animación de ejecución por un evento de animación
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.executionTarget = null;
        player.comboBarAmount = 0f;
    }
}
