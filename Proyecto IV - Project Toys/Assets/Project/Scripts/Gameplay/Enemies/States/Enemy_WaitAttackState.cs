using UnityEngine;

public class Enemy_WaitAttackState : EnemyState
{
    private Vector3 attackPoint;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private float currentTime;
    //private bool hasStartedAttack;
    public Enemy_WaitAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.isAttacking = true;

        // Guardar posiciones
        currentPosition = enemy.transform.position;
        lastPlayerPosition = enemy.playerTransform != null ? enemy.playerTransform.position : enemy.transform.forward;

        // Calcular punto de ataque (hacia donde va a embestir)
        Vector3 directionToPlayer = (lastPlayerPosition - currentPosition).normalized;
        attackPoint = currentPosition + directionToPlayer * enemy.attackRange;
        enemy.agent.destination = currentPosition + (lastPlayerPosition - currentPosition) * 0.25f;
        //attackPoint = lastPlayerPosition;

        // DETENER al enemigo durante la carga del ataque
        //enemy.agent.isStopped = true;
        //enemy.agent.velocity = Vector3.zero;

        // Mirar hacia el objetivo
        enemy.transform.LookAt(new Vector3(attackPoint.x, enemy.transform.position.y, attackPoint.z), Vector3.up);

        // Animación de carga
        //anim.Play("WaitAttack");

        currentTime = 0;
        //enemy.hasStartedAttack = false;
    }


    public override void Update()
    {
        base.Update();

        currentTime += Time.deltaTime;
        //if (currentTime >= enemy.waitTime && !hasStartedAttack)
        if (currentTime >= enemy.waitTime)
        {
            Debug.Log("Cambiar a estado de ataque");
            enemy.attackState.SetParametersAttack(currentPosition, attackPoint);
            
            stateMachine.ChangeState(enemy.attackState);
        }

    }
    public override void Exit()
    {
        base.Exit();
        enemy.agent.isStopped = false;
    }
}
