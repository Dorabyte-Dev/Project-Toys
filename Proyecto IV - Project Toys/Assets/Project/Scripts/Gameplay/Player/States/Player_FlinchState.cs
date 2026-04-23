using UnityEngine;

public class Player_FlinchState : PlayerState
{
    public Player_FlinchState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, "Flinch")
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = player.anim.GetCurrentAnimatorClipInfo(0).Length; // Duración de la animación de flinch
    }

    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
    }
}
