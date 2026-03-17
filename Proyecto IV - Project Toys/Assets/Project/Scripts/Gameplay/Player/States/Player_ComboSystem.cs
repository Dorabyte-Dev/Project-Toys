using System;
using UnityEngine;

public class Player_ComboSystem : PlayerState
{
    
    public bool isLightAttack;
    public float attackInitialPlayerAngle;
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
        RotateWithinCombo();

        if (player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= player.comboBufferUnlockThreshold)
        {
            if (input.Player.LightAttack.WasPressedThisFrame())
                QueueNextAttack(AttackType.Light);
        
            if(input.Player.HeavyAttack.WasPressedThisFrame())
                QueueNextAttack(AttackType.Heavy);
        }

        if (input.Player.Dash.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.dashState);
            player.OnComboInterrupted();
        }
    }

    private void RotateWithinCombo()
    {
        if(player.cameraMoveInput.magnitude >= 0.1f)
        {
            float inputAngle = Mathf.Atan2(player.cameraMoveInput.x, player.cameraMoveInput.y) * Mathf.Rad2Deg;
            
            float angleDiff = Mathf.DeltaAngle(attackInitialPlayerAngle, inputAngle);
            float clampedDiff = Mathf.Clamp(angleDiff, -player.comboRedirectionLimit, player.comboRedirectionLimit);
            
            float targetAngle = attackInitialPlayerAngle + clampedDiff;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            
            //Quaternion finalRotation = Quaternion.Slerp(player.transform.rotation, targetRotation, player.turnSmoothVelocity * Time.deltaTime);

            player.transform.rotation = targetRotation;
        }
    }

    private void ApplyAttackVelocity()
    {
        Vector3 finalMoveDirection = player.transform.forward * player.currentAttack.attackMoveDistance;
        float animTime = player.anim.GetCurrentAnimatorClipInfo(0).Length;
        float attackMoveDuration = (player.currentAttack.attackMoveDurationEnd - player.currentAttack.attackMoveDurationStart) * animTime;
        
        Vector3 attackVelocity = finalMoveDirection / attackMoveDuration;
        
        float normalizedTime = player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (normalizedTime > player.currentAttack.attackMoveDurationStart
            && normalizedTime < player.currentAttack.attackMoveDurationEnd)
        {
            player.SetVelocity(attackVelocity.x, attackVelocity.z);
            Debug.DrawLine(player.transform.position, player.transform.position + player.transform.forward * player.currentAttack.attackMoveDistance, Color.red);
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
