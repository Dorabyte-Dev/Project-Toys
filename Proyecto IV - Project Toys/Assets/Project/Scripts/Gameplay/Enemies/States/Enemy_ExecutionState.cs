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
        enemy.Execution_Enter();
        enemy._vfx.ResetPushFeedback();
        enemy.agent.ResetPath();
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        enemy.isBeingExecuted = true;
        enemy.transform.DODynamicLookAt(enemy.playerTransform.position, 0.5f, AxisConstraint.Y);
    }

    public override void Update()
    {
        base.Update();
        /*enemy.gameObject.transform.DOShakeScale(1f, 0.1f, 5).OnComplete(() =>
        {
            stateMachine.ChangeState(enemy.deadState);
        });*/
        enemy.Execution_Update();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.Execution_Exit();
        Debug.LogWarning("Exiting Execution State.");
        enemy.isBeingExecuted = false;
    }
}
