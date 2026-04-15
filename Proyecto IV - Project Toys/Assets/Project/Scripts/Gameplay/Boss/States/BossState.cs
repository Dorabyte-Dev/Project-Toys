using UnityEngine;

public class BossState : EntityState
{
    protected Boss boss;


    public BossState(Boss boss, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.boss = boss;
        
        anim = boss.anim;
    }
}
