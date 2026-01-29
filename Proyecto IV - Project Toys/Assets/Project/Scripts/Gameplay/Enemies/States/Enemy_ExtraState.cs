public class Enemy_ExtraState : EnemyState
{
    public Enemy_ExtraState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        enemy.Extra_Enter();
    }

    public override void Update()
    {
        base.Update();
        
        enemy.Extra_Update();
    }
    public override void Exit()
    {
        base.Exit();
        
        enemy.Extra_Exit();
    }
}