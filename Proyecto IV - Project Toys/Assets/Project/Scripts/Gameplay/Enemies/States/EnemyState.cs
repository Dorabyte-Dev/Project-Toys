using System.Collections;
using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;
    protected Rigidbody rb;

    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        // Referencia del enemigo
        this.enemy = enemy;
        
        // Referencias comunes
        rb = enemy.rb;
        anim = enemy.anim;
    }
    
    protected void GetDistanceToPlayer()
    {
        enemy.distanceToPlayer = enemy.CheckPlayerDistance();
    }
    
    
    // protected IEnumerator GetDistanceToPlayer(float waitTime)
    // {
    //     enemy.distanceToPlayer = enemy.CheckPlayerDistance();
    //     yield return new WaitForSeconds(waitTime);
    // }
}
