using UnityEngine;

public class BossState : EntityState
{
    protected Boss boss;


    public BossState(Boss boss, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.boss = boss;
        
        anim = boss.anim;
    }

    public void RandomAttackState()
    {
        int randomAttack = Random.Range(0, 2);
        if (randomAttack == 0)
        {
            //Cambiar al estado de ataque a distancia
            boss.ChangeBossState(boss.rangedAttackState);
        }
        else
        {
            //Cambiar al estado de ataque cuerpo a cuerpo
            boss.ChangeBossState(boss.meleeAttackState);
        }
    }
}
