using UnityEngine;

public class Boss_SlamAttackState : BossState
{
    public Boss_SlamAttackState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 3f;
        boss.isAttacking = true;
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
