using UnityEngine;

public class Player_HeavyAttackState
{
    /*private float heavyAttackVelocityTimer;
    private float lastTimeHeavyAttacked;

    private bool heavyComboAttackQueued;
    private int heavyComboIndex = 1;
    private int heavyComboLimit = 3;
    private const int firstHeavyComboIndex = 1;

    public Player_HeavyAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if(heavyComboLimit != player.attackVelocity.Length)
        {
            heavyComboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();
        heavyComboAttackQueued = false;
        
        ResetComboIndexIfNeeded();

        anim.SetInteger("heavyAttackIndex", heavyComboIndex);
        ApplyAttackVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        heavyComboIndex++;
        lastTimeHeavyAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.LightAttack.WasPressedThisFrame())
           QueueNextAttack();

        if (triggerCalled)
        {
            if (heavyComboAttackQueued)
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
        if (heavyComboIndex < heavyComboLimit)
            heavyComboAttackQueued = true;
    }

    private void HandleAttackVelocity()
    {
        heavyAttackVelocityTimer -= Time.deltaTime;

        if(heavyAttackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {   
        Vector2 attackVelocity = player.attackVelocity[heavyComboIndex - 1];

        heavyAttackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x, attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        if(Time.time > lastTimeHeavyAttacked + player.comboResetTime)
            heavyComboIndex = firstHeavyComboIndex;

        if (heavyComboIndex > heavyComboLimit)
            heavyComboIndex = firstHeavyComboIndex;
    }*/


}
