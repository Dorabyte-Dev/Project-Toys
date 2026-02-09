using UnityEngine;

public class Player_ComboSystem : PlayerState
{

    private float AttackVelocityTimer;
    private float lastTimeAttacked;

    private bool ComboAttackQueued;
    private int ComboIndex = 1;
    private int ComboLimit = 3;
    private const int firstComboIndex = 1;

    //bools
    public bool isHeavy;
    public string attackType;


    public Player_ComboSystem(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (ComboLimit != player.attackVelocity.Length)
        {
            ComboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();
        ComboAttackQueued = false;

        ResetComboIndexIfNeeded();

        if(isHeavy)
        {
            attackType = "HeavyPressed";
        }
        else if (!isHeavy)
        {
            attackType = "LightPressed";
        }
        anim.SetInteger("AttackIndex", ComboIndex);
        ApplyAttackVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        ComboIndex++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.LightAttack.WasPressedThisFrame())
            QueueNextAttack();

        if (triggerCalled)
        {
            if (ComboAttackQueued)
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
        if (ComboIndex < ComboLimit)
            ComboAttackQueued = true;
    }

    private void HandleAttackVelocity()
    {
        AttackVelocityTimer -= Time.deltaTime;

        if (AttackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[ComboIndex - 1];

        AttackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x, attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        if (Time.time > lastTimeAttacked + player.comboResetTime)
            ComboIndex = firstComboIndex;

        if (ComboIndex > ComboLimit)
            ComboIndex = firstComboIndex;
    }
}
