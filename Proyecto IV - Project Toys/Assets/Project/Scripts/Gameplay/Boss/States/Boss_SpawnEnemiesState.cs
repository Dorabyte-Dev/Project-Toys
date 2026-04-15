using UnityEngine;

public class Boss_SpawnEnemiesState : BossState
{
    public Boss_SpawnEnemiesState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if(boss.enemySpawner == null)
        {
            Debug.LogError("Boss_SpawnEnemiesState: Boss does not have an EnemySpawner component.");
            boss.ChangeBossState(boss.idleState);
            return;
        }
        
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
