using UnityEngine;

public class Enemy_MoveState : EnemyState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }


    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(enemy.moveSpeed * 1f,  enemy.moveSpeed * 1f);

        if (enemy.groundDetected == false)
            stateMachine.ChangeState(enemy.idleState);
            enemy.transform.Rotate(0f, -180f, 0f);
            enemy.SetVelocity(enemy.moveSpeed * 1f, enemy.moveSpeed * 1f);
    }
}
