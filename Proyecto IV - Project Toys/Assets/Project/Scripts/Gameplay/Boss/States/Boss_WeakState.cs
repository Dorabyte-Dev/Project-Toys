using UnityEngine;

public class Boss_WeakState : BossState
{
    //private float stateTimer;
    public Boss_WeakState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        boss.canBeDamaged = true;
        stateTimer = boss.timeInWeakState;
    }

    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            boss.ChangeBossState(boss.spawnEnemiesState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        boss.canBeDamaged = false;
    }
}
