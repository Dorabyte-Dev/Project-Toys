using UnityEngine;


public class Player_LightAttackState : PlayerState
{
    private float attackVelocityTimer;


    private const int firstComboIndex = 1;
    private int comboIndex = 1;
    private int comboLimit = 3;

    private float lastTimeAttacked;

    public Player_LightAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        
        ResetComboIndexIfNeeded();

        anim.SetInteger("lightAttackIndex", comboIndex);
        ApplyAttackVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        comboIndex++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if(triggerCalled) 
            stateMachine.ChangeState(player.idleState);
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {
        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(player.attackVelocity.x, player.attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        if(Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = firstComboIndex;

        

        if (comboIndex > comboLimit)
            comboIndex = firstComboIndex;
    }

}
