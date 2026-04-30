using UnityEngine;
using UnityEngine.AI;

public class Enemy_WaitAttackState : EnemyState
{
    public Enemy_WaitAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        enemy.WaitAttack_Enter();
    }
    
    public override void Update()
    {
        base.Update();
        enemy.WaitAttack_Update();
    }
    public override void Exit()
    {
        base.Exit();
        enemy.WaitAttack_Exit();
    }
}
