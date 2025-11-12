using UnityEngine;


public class Player_LightAttackState : PlayerState
{
    private float attackVelocityTimer;
    private float lastTimeAttacked;

    private bool comboAttackQueued;
    private int comboIndex = 1;
    private int comboLimit = 3;
    private const int firstComboIndex = 1;


    public Player_LightAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if(comboLimit != player.attackVelocity.Length)
        {
            comboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        
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

        if (input.Player.LightAttack.WasPressedThisFrame())
           QueueNextAttack();

        if(triggerCalled)
        {
            if (comboAttackQueued)
            {
                anim.SetBool(animBoolName, false);
               player.EnterAttackStateWithDelay();
           }
            else
                stateMachine.ChangeState(player.idleState);
        }
    }

    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
            comboAttackQueued = true;
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {   
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x, attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        if(Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = firstComboIndex;

        if (comboIndex > comboLimit)
            comboIndex = firstComboIndex;
    }

}
