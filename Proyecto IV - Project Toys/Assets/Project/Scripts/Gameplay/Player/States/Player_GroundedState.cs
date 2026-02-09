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

        if(rb.linearVelocity.y < 0 && !player.groundDetected)
            stateMachine.ChangeState(player.fallState);

        if (input.Player.Jump.WasPerformedThisFrame())
            stateMachine.ChangeState(player.jumpState);

        if (input.Player.Dash.WasPerformedThisFrame() && player.CanDash())
            stateMachine.ChangeState(player.dashState);

        /*if (input.Player.LightAttack.WasPerformedThisFrame())
            stateMachine.ChangeState(player.lightAttackState);
        
<<<<<<< Updated upstream
        /*if(input.Player.HeavyAttack.WasPerformedThisFrame())
            stateMachine.ChangeState(player.heavyState);
        */
        if (input.Player.HeavyAttack.WasPerformedThisFrame())
            //stateMachine.ChangeState(/* Combo System State*/);
=======
        if(input.Player.HeavyAttack.WasPerformedThisFrame())
            stateMachine.ChangeState(player.heavyState);*/

        if (input.Player.LightAttack.WasPerformedThisFrame())
        {
            player.comboSystem.isHeavy = false;
            stateMachine.ChangeState(player.comboSystem);
        }

        if(input.Player.HeavyAttack.WasPerformedThisFrame())
        {
            player.comboSystem.isHeavy = true;
            stateMachine.ChangeState(player.comboSystem);
        }
>>>>>>> Stashed changes

        if (input.Player.Execution.WasPerformedThisFrame())
            if (player.CanExecute())
            {
                player.executionEnemy.ChangeEnemyState(player.executionEnemy.executionState);
                stateMachine.ChangeState(player.executionState);
            } 
            else
                Debug.Log("Cannot execute now.");
        
    }
    public override void Exit()
    {
        base.Exit();
    }
}
