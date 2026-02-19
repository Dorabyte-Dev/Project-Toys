using UnityEngine;

public class Player_GroundedState : PlayerState
{

    public Player_GroundedState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        /*if(rb.linearVelocity.y < 0 && !player.groundDetected)
            stateMachine.ChangeState(player.fallState);*/

        /*if (input.Player.Jump.WasPerformedThisFrame())
            stateMachine.ChangeState(player.jumpState);*/

        if (input.Player.Dash.WasPerformedThisFrame() && player.CanDash())
            stateMachine.ChangeState(player.dashState);

        if (input.Player.LightAttack.WasPerformedThisFrame())
        {
            stateMachine.ChangeState(player.comboSystemState);
            Debug.Log("Light Attack Performed");
            player.CheckAttackBuffer(true);
            
        }

        if (input.Player.HeavyAttack.WasPerformedThisFrame())
        {
            stateMachine.ChangeState(player.comboSystemState);
            player.CheckAttackBuffer(false);
        }
        

        if (input.Player.Execution.WasPerformedThisFrame())
        {
            if (player.CanExecute())
            {
                player.executionEnemy.ChangeEnemyState(player.executionEnemy.executionState);
                stateMachine.ChangeState(player.executionState);
            } 
            else
                Debug.Log("Cannot execute now.");
        }
        
    }
    public override void Exit()
    {
        base.Exit();
    }
}
