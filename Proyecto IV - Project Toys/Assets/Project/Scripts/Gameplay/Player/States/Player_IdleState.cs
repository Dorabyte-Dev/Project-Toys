using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player has entered Idle State.");
        player.SetVelocity(0,0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.cameraMoveInput != Vector2.zero && player.canMove)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
