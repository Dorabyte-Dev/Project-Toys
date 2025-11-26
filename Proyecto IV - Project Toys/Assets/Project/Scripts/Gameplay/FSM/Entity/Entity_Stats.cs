using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat vitality;
    public Stat attackDamage;
    public Stat heavyDamage;

    public float GetMaxetHealth()
    {
        float baseHp = maxHealth.GetValue();
        float vitBonus = vitality.GetValue() * 5f;

        return baseHp + vitBonus;
    }

    public float GetMaxAttack()
    {
        float damage = attackDamage.GetValue();
        return damage;
    }
    
    public float GetMaxHeavyAttack()
    {
        float heavyAttackDamage = heavyDamage.GetValue();
        return heavyAttackDamage;
    }

}
