public class Enemy_Combat : Entity_Combat
{
    public override void PerformAttack()
    {
        base.PerformAttack();
    }
    
    public float GetBaseDamage()
    {
        return baseDamage;
    }
}