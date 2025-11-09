using UnityEngine;

public class Enemy_Health : Entity_Health
{
    public override void TakeDamage(float takeDamage, Transform damageDealer)
    {
        base.TakeDamage(takeDamage, damageDealer);
    }

}
