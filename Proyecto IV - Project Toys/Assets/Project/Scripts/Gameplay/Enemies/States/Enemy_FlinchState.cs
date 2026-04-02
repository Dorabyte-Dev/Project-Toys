using UnityEngine;

public class Enemy_FlinchState : EnemyState
{
    public Enemy_FlinchState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.Flinch_Enter();
        enemy.isFlinching = true;
        //enemy._vfx.ResetPushFeedback();
    }

    public override void Update()
    {
        base.Update();
        enemy.Flinch_Update();
    }
    public override void Exit()
    {
        base.Exit();
        enemy.Flinch_Exit();
        enemy.isFlinching = false;
    }

}
