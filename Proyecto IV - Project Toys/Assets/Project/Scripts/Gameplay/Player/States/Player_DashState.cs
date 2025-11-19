using DG.Tweening;
using UnityEngine;

public class Player_DashState : Player_GroundedState
{
    Vector3 forToApply;
    float dashSpeed;
    bool enteredSlope;
    bool switchSlope;
    bool isPerfectDodge;
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = player.dashDuration;

        PerfectDodgeManager gm = Object.FindAnyObjectByType<PerfectDodgeManager>();
        isPerfectDodge = gm.pDodgeEnemies.Count > 0;
        if (isPerfectDodge)
        {
            PerfectDodge(gm.pDodgeEnemies[gm.pDodgeEnemies.Count - 1]);
        }
        else
        {
            Vector3 playerDirection = GetDashDirection();
            Debug.Log("Dash direction: " + playerDirection);

            dashSpeed = player.dashDistance / player.dashDuration;
            forToApply = playerDirection * dashSpeed;
            rb.AddForce(forToApply, ForceMode.VelocityChange);

            enteredSlope = player.OnSlope();
            Debug.Log("Dash applied: " + forToApply);
        }
        
    }


    public override void Update()
    {
        base.Update();

        
        if(enteredSlope != player.OnSlope() && !switchSlope)
        {
            if (enteredSlope)
            {
                float remainingDashSpeed = rb.linearVelocity.magnitude;
                Vector3 newDashDirection = player.ProjectVectorOutOfSlope(rb.linearVelocity).normalized;
                rb.linearVelocity = newDashDirection * remainingDashSpeed;
            }
            else 
            {
                Debug.Log("My last Linear velocity is: " + rb.linearVelocity + ", with magnitude of " + rb.linearVelocity.magnitude);
                float remainingDashSpeed = rb.linearVelocity.magnitude;
                Vector3 newDashDirection = player.ProjectVectorOnSlope(rb.linearVelocity).normalized;
                rb.linearVelocity = newDashDirection * remainingDashSpeed;
                Debug.Log("My new Linear velocity is: " + rb.linearVelocity + ", with magnitude of " + rb.linearVelocity.magnitude);
            }
        }
        if (stateTimer < 0f)
        {
            if (player.groundDetected)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.fallState);
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0f, 0f);
    }

    private Vector3 GetDashDirection()
    {
        Vector2 inputVector = player.cameraMoveInput;
        if (inputVector.magnitude < 0.1f)
            return player.transform.forward;

        Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y).normalized;
        if(player.OnSlope()) 
            direction = player.ProjectVectorOnSlope(direction);
        return direction;
    }

    void PerfectDodge(GameObject enemy)
    {
        Debug.LogWarning("Perfect");

        //Calculate position to move
        Vector3 direction =  enemy.transform.position - player.transform.position;
        Vector3 pDodgePosition = enemy.transform.position + direction.normalized * player.perfectDodgeEnemyDistance;
        Vector3 pDodgeCurvePoint = player.transform.position + direction / 2 + Vector3.Cross(direction, Vector3.up).normalized * direction.magnitude / 1.25f;

        Vector3[] curvePoints = new Vector3[] { player.transform.position, pDodgeCurvePoint, pDodgePosition };
        Camera.main.fieldOfView = 30;
        Time.timeScale = 0.25f;

        if(player.afterimageTrail != null)
        {
            player.afterimageTrail.toggleTrail = true;
        }
        rb.DOPath(curvePoints, player.dashDuration).OnComplete(() => { Time.timeScale = 1f; Camera.main.fieldOfView = 60; });
        

    }
}
