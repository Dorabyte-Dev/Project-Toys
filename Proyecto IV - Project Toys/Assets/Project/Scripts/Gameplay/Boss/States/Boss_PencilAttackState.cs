using System.Collections.Generic;
using UnityEngine;

public class Boss_PencilAttackState : BossState
{
    public Boss_PencilAttackState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 3f;
        boss.isAttacking = true;
        if (boss._projectiles == null) boss._projectiles = new List<GameObject>(boss.numberOfPencilsToInvoke);
        
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer <= 0 && !boss.isAttacking)
        {
            stateMachine.ChangeState(boss.baseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    
}
