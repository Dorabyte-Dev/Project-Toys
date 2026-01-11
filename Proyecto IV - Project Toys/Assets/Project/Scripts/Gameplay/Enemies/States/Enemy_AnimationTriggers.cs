using UnityEngine;

public class Enemy_AnimationTriggers : Entity_AnimationTriggers
{
    [SerializeField]private Enemy enemy;

    public override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }

    public void NotifyAttackFinished()
    {
        enemy.isAttacking = false;
    }

    public override void AttackTrigger()
    {
        if(enemy.hasAttacked) return;
        Debug.LogError("Enemy Attack Triggered");
        base.AttackTrigger();
    }

    public override void HeavyTrigger()
    {
        if(enemy.hasAttacked) return;
        base.HeavyTrigger();
    }
}
