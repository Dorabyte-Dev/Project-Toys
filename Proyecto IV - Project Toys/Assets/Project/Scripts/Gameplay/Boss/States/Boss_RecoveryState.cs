using UnityEngine;

public class Boss_RecoveryState : BossState
{
    public Boss_RecoveryState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    private void GoToArena()
    {
        boss.agent.SetDestination(boss.arenaCenterTransform.position);
    }
}
