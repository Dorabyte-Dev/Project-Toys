using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player has entered Move State.");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }
        //RotatePlayerToMatchInput();
        
        player.SetVelocity(player.moveInput.x * player.moveSpeed, player.moveInput.y * player.moveSpeed);
    }

    Vector2 MovementDirectionToCamera(Vector2 moveInput)
    {
        Vector2 moveVector;
        Vector3 xVector;
        Vector3 yVector;
        
        
        
        
        return moveVector;
    }
    //public void RotatePlayerToMatchInput()
    //{
    //    Vector2 direction = new Vector2(player.moveInput.x, player.moveInput.y).normalized;

    //    if (direction.magnitude >= 0.1f)
    //    {
    //        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
    //        float angle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref player.turnSmoothVelocity, player.turnSmoothTime);
    //        player.transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    //    }
    //}
}
