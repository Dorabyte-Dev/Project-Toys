using DG.Tweening;
using UnityEngine;

public class Enemy_ExecutionState : EnemyState
{
    public Enemy_ExecutionState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered Enemy_ExecutionState");
        enemy.agent.isStopped = true;
    }

    public override void Update()
    {
        base.Update();
        enemy.gameObject.transform.DOShakeScale(1f, 0.1f, 5).OnComplete(() =>
        {
            stateMachine.ChangeState(enemy.deadState);
        });
    }

    public override void Exit()
    {
        base.Exit();
    }
}
