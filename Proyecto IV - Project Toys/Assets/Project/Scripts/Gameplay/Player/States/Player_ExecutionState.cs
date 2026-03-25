using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class Player_ExecutionState : PlayerState
{
    public Player_ExecutionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    //private float executionDuration = 5f; //La idea es que se cambie luego cuando esté la animación de ejecución por un evento de animación
    private CinemachineCamera executionCamera;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered Player_ExecutionState");
        //stateTimer = executionDuration;  //La idea es que se cambie luego cuando esté la animación de ejecución por un evento de animación
        //CameraManager.instance.ToggleZoom();
        GoToExecutionPoint();
        //player.transform.LookAt(new Vector3(player.executionEnemy.transform.position.x, player.executionEnemy.transform.position.y, player.transform.position.z));
        player.transform.DODynamicLookAt(player.executionEnemy.transform.position, 0.5f, AxisConstraint.Y).OnComplete((
            () =>
            {
                player.transform.DOMove(player.executionEnemy.playerExecutionTransform.position, 0.25f);
            }));
        executionCamera = player.executionCameraManager.TestExecutionCamera();
        executionCamera.Priority = 100;
        
        
        
        player.executionTarget = null;
        player.comboBarAmount = 0f;
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
        
        //CameraManager.instance.UntoggleZoom();
        if (executionCamera != null)
            executionCamera.Priority = 0;
    }
    
    private void GoToExecutionPoint()
    {
        Vector3 directionToEnemy = player.executionEnemy.transform.position - player.transform.position;
        Vector3 executionPoint = player.transform.position + directionToEnemy.normalized * (directionToEnemy.magnitude - 1f);
        player.transform.position = executionPoint;
    }
}
