using UnityEngine;
using UnityEngine.AI;

public class Enemy_MoveState : EnemyState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GoToRandomPoint();
    }


    public override void Update()
    {
        base.Update();

        if (!enemy.groundDetected)
        {
            stateMachine.ChangeState(enemy.idleState);
            enemy.Flip();
        }
    }

    public void GoToRandomPoint()
    {
        if (enemy.agent.pathPending || !enemy.agent.isOnNavMesh || enemy.agent.remainingDistance > 0.1f)
            return;

        enemy.agent.destination = RandomNavSphere(enemy.transform.position, enemy.range, -1);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
