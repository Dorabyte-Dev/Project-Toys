using UnityEngine;
using UnityEngine.AI;

public class Enemy_WaitAttackState : EnemyState
{
    private Vector3 attackPoint;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private Vector3 directionToPlayer;
    private float currentTime;
    //private bool hasStartedAttack;

    
    public Enemy_WaitAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //enemy.isAttacking = true;

        // Guardar posiciones
        currentPosition = enemy.transform.position;
        lastPlayerPosition = enemy.playerTransform != null ? enemy.playerTransform.position : enemy.transform.forward;

        // Calcular punto de ataque (hacia donde va a embestir)
        
        StopSmooth();
        //attackPoint = lastPlayerPosition;

        // DETENER al enemigo durante la carga del ataque
        //enemy.agent.isStopped = true;
        //enemy.agent.velocity = Vector3.zero;

        // Mirar hacia el objetivo
        LookToPlayer();

        // Animaci�n de carga
        //anim.Play("WaitAttack");

        currentTime = 0;
        //enemy.hasStartedAttack = false;
        
        
        enemy.orbitAngle = InitialiceOrbitAngle();
    }

    private void LookToPlayer()
    {
        enemy.transform.LookAt(new Vector3(enemy.playerTransform.position.x, enemy.transform.position.y, enemy.playerTransform.position.z), Vector3.up);
    }

    public override void Update()
    {
        base.Update();
        enemy.canAttackByManager = EnemyWaveManager.Instance.RequestAttackPermission(enemy);
        currentTime += Time.deltaTime;
        //if (currentTime >= enemy.waitTime && !hasStartedAttack)
        OrbitAroundPlayer();
        LookToPlayer();
        if (currentTime >= enemy.waitTime)
        {
            if (enemy.canAttackByManager)
            {
                //Debug.Log("Cambiar a estado de ataque");
                attackPoint = AttackPointToPlayer();
                enemy.attackState.SetParametersAttack(currentPosition, attackPoint);
            
                stateMachine.ChangeState(enemy.attackState);
            }
            else
            {
                currentTime = 0;
            }
            
        }

    }
    public override void Exit()
    {
        base.Exit();
        enemy.agent.isStopped = false;
    }
    private void StopSmooth()
    {
        enemy.agent.destination = currentPosition + (lastPlayerPosition - currentPosition) * 0.25f;
    }

    private Vector3 AttackPointToPlayer()
    {
        directionToPlayer = (enemy.playerTransform.position - enemy.transform.position).normalized;
        return enemy.transform.position + directionToPlayer * enemy.attackRange;
    }

    private void OrbitAroundPlayer()
    {
        // 1. Aumentar el �ngulo de �rbita con el tiempo
        // El Time.deltaTime * OrbitSpeed hace que el punto rote.
        enemy.orbitAngle += Time.deltaTime * enemy.orbitSpeed;

        // Asegurar que el �ngulo no se desborde (opcional, por limpieza)
        if (enemy.orbitAngle > 360f)
        {
            enemy.orbitAngle -= 360f;
        }

        // 2. Convertir el �ngulo a radianes para las funciones trigonom�tricas
        // Los �ngulos en C# suelen ser en grados.
        float angleInRad = enemy.orbitAngle * Mathf.Deg2Rad;

        // 3. Calcular la nueva posici�n de destino (en un plano 2D, X y Z)
        Vector3 targetPosition;
        targetPosition.x = enemy.playerTransform.position.x + enemy.orbitDistance * Mathf.Cos(angleInRad);
        targetPosition.y = enemy.playerTransform.position.y; // Mantener la altura del suelo
        targetPosition.z = enemy.playerTransform.position.z + enemy.orbitDistance * Mathf.Sin(angleInRad);


        // 4. Mover el NavMeshAgent al nuevo destino
        enemy.agent.destination = targetPosition;
    }

    private float InitialiceOrbitAngle()
    {
        Vector3 directionPlayerToEnemy = enemy.transform.position - enemy.playerTransform.position;
        float angleInRadians = Mathf.Atan2(directionPlayerToEnemy.z, directionPlayerToEnemy.x);
        return angleInRadians * Mathf.Rad2Deg;
    }
}
