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
}
