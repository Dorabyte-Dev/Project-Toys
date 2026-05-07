using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class Player_ExecutionState : PlayerState
{
    public Player_ExecutionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    private CinemachineCamera executionCamera;
    public override void Enter()
    {
        base.Enter();
        CheckEnemyType();
        Debug.Log("Entered Player_ExecutionState");
        //stateTimer = executionDuration;  //La idea es que se cambie luego cuando esté la animación de ejecución por un evento de animación
        GoToExecutionPoint();
        player.transform.DODynamicLookAt(player.executionEnemy.transform.position, 0.25f, AxisConstraint.Y).OnComplete((
            () =>
            {
                player.transform.DOMove(player.executionTransform.position, 0.25f);
            }));
        player.executionCameraManager.MoveCamera(player.executionTransform.position, -player.executionTransform.forward);
        //player.executionCameraManager.transform.parent = null;
        executionCamera = player.executionCameraManager.GetAvailableCameraRaycastAndCameraProximity(player.gameObject);
        executionCamera.Priority = 100;
        
        
        
        player.executionTarget = null;
        player.comboBarAmount = 0f;

        player._health.invincibleMode = true;
    }

    public override void Update()
    {
        base.Update();
        /*if (stateTimer <= 0f)   //La idea es que se cambie luego cuando esté la animación de ejecución por un evento de animación
        {
            stateMachine.ChangeState(player.idleState);
        }*/
    }

    public override void Exit()
    {
        base.Exit();
        player._health.invincibleMode = false;

        if (executionCamera != null)
        {
            //player.executionCameraManager.BackToDefaultCamera();
            executionCamera.Priority = -100;
        }
    }
    
    private void GoToExecutionPoint()
    {
        Vector3 directionToEnemy = player.executionEnemy.transform.position - player.transform.position;
        Vector3 executionPoint = player.transform.position + directionToEnemy.normalized * (directionToEnemy.magnitude - 1f);
        player.transform.position = executionPoint;
    }

    private void CheckEnemyType()
    {
        int enemyTypeIndex;

        switch (player.executionEnemy.enemyType)
        {
            case Enemy.EnemyType.Melee:
                enemyTypeIndex = 1;
                break;
            case Enemy.EnemyType.Ranged:
                enemyTypeIndex = 2;
                break;
            case Enemy.EnemyType.Golem:
                enemyTypeIndex = 3;
                break;
            case Enemy.EnemyType.MiniGolem:
                enemyTypeIndex = 1;
                break;
            default:
                Debug.LogWarning("Unknown enemy type during execution.");
                enemyTypeIndex = 0;
                break;
        }
        
        player.anim.SetInteger("executionIndex", enemyTypeIndex);
    }
}
