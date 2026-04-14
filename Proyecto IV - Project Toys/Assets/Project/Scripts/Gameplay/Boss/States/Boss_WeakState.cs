using UnityEngine;

public class Boss_WeakState : BossState
{
    public Boss_WeakState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        boss.canBeDamaged = true;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        boss.canBeDamaged = false;
    }
}
