using System;
using UnityEngine;

public class Player_ComboSystem : PlayerState
{

    /*private float AttackVelocityTimer;
    private float lastTimeAttacked;

    private bool ComboAttackQueued;*/
    //private int ComboIndex = 1;
    //private int ComboLimit = 3;
    //private const int firstComboIndex = 1;

    //bools
    public bool isLightAttack;
    private float attackTimer;
    private enum AttackType
    {
        Light,
        Heavy
    }


    public Player_ComboSystem(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        /*if (ComboLimit != player.attackVelocity.Length)
        {
            ComboLimit = player.attackVelocity.Length;
        }*/
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0);
        Debug.Log("Player has entered Combo System State.");
        player.OnComboStarted();
        if (player.currentAttack != null)
        {
            AttackData nextAttack = isLightAttack ? player.currentAttack.nextLightAttack : player.currentAttack.nextHeavyAttack;
            if (nextAttack != null)
            {
                anim.CrossFade(nextAttack.name, .25f /*player.attackTransitionDuration*/);
            }
        }
        else
        {
        }
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        
        base.Update();
        //player.SetVelocity(0,0);
        ApplyAttackVelocity();

        if (player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= player.comboBufferUnlockThreshold)
        {
            if (input.Player.LightAttack.WasPressedThisFrame())
                QueueNextAttack(AttackType.Light);
        
            if(input.Player.HeavyAttack.WasPressedThisFrame())
                QueueNextAttack(AttackType.Heavy);
        }
        
        
    }
    
    private void ApplyAttackVelocity()
    {
        
        float normalizedTime = player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (normalizedTime > player.currentAttack.attackVelocityDurationStart
            && normalizedTime < player.currentAttack.attackVelocityDurationEnd)
        {
            Vector2 attackVelocity = player.currentAttack.attackVelocity * player.transform.forward;
            player.SetVelocity(attackVelocity.x, attackVelocity.y);
        }
        else
        {
            player.SetVelocity(0, 0);
        }
        
    }
    
    public void Test()
    {
        Debug.Log("My motionValue is " +  player.currentAttack.motionValue);
    }

    private void QueueNextAttack(AttackType type)
    {
        switch (type)
        {
            case AttackType.Light:
                anim.SetTrigger("LightTrigger");
                break;
            case AttackType.Heavy:
                anim.SetTrigger("HeavyTrigger");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    #region Crap

    /*private void LoadNextAttack(AttackData data)
    {
        currentAttackData = data;
        anim.CrossFade(currentAttackData.attackName, 1); //Make 1 a variable in Player or AttackData
    }*/
    
    private void HandleAttackVelocity()
    {
        /*AttackVelocityTimer -= Time.deltaTime;

        if (AttackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);*/
    }

    private void ResetComboIndexIfNeeded()
    {
        /*if (Time.time > lastTimeAttacked + player.comboResetTime)
            ComboIndex = firstComboIndex;

        if (ComboIndex > ComboLimit)
            ComboIndex = firstComboIndex;*/
    }

    #endregion
}
