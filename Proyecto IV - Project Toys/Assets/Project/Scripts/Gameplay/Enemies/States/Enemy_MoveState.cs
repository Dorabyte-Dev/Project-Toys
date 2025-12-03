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
        GetDistanceToPlayer();
        if (!enemy.groundDetected)
        {
            stateMachine.ChangeState(enemy.idleState);
            enemy.Flip();
            return;
        }

        if (enemy.distanceToPlayer < enemy.pursuitPlayerRange)
        {
            stateMachine.ChangeState(enemy.pursuitState);
        }
        // Verificar si ha llegado al destino
        if (HasReachedDestination())
        {
            stateMachine.ChangeState(enemy.idleState);
            //GoToRandomPoint();
        }
    }

    

    public void GoToRandomPoint()
    {
        if (enemy.agent.pathPending || !enemy.agent.isOnNavMesh)
            return;

        enemy.agent.destination = RandomNavSphere(enemy.transform.position, enemy.range, -1);
    }

    private bool HasReachedDestination()
    {
        if (!enemy.agent.pathPending)
        {
            if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
            {
                if (!enemy.agent.hasPath || enemy.agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
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