using UnityEngine;

public class Player_Health : Entity_Health
{
    [SerializeField] private float healAmount;
    public virtual void Heal()
    {
        currentHp += healAmount;
        if (currentHp > maxHp)
            currentHp = maxHp;
        //Play heal VFX here:
            //Green Particles going up
            //Heal sound effect
            //Player flash green
    }
}