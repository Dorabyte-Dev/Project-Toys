using UnityEngine;


public class Player_LightAttackState
{
    /*private float lightAttackVelocityTimer;
    private float lastTimeLightAttacked;

    private bool lightComboAttackQueued;
    private int lightComboIndex = 1;
    private int lightComboLimit = 3;
    private const int firstLightComboIndex = 1;


    public Player_LightAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if(lightComboLimit != player.attackVelocity.Length)
        {
            lightComboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();
        lightComboAttackQueued = false;
        
        ResetComboIndexIfNeeded();

        anim.SetInteger("lightAttackIndex", lightComboIndex);
        ApplyAttackVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        lightComboIndex++;
        lastTimeLightAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.LightAttack.WasPressedThisFrame())
           QueueNextAttack();

        if (triggerCalled)
        {
            if (lightComboAttackQueued)
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
        if (lightComboIndex < lightComboLimit)
            lightComboAttackQueued = true;
    }

    private void HandleAttackVelocity()
    {
        lightAttackVelocityTimer -= Time.deltaTime;

        if(lightAttackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {   
        Vector2 attackVelocity = player.attackVelocity[lightComboIndex - 1];

        lightAttackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x, attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        if(Time.time > lastTimeLightAttacked + player.comboResetTime)
            lightComboIndex = firstLightComboIndex;

        if (lightComboIndex > lightComboLimit)
            lightComboIndex = firstLightComboIndex;
    }*/

}
