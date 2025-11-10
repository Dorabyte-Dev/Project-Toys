using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat vitality;
    public Stat attackDamage;

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

}
