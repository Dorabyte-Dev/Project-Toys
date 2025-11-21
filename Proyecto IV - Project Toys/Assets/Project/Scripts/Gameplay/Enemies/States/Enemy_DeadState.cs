using UnityEngine;

public class Enemy_DeadState : EnemyState
{

    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        anim.enabled = false;
        enemy.agent.enabled = false;
        if (enemy.spawner != null)
            enemy.spawner.EnemyDead(enemy.gameObject);
        stateMachine.SwitchOffStateMachine();
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
