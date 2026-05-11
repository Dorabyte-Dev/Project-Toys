using UnityEngine;

public class Boss_Combat : Entity_Combat
{
    private Boss boss;
    public float slamDamage = 20f;
    public float pencilDamage = 10f;
    
    public override void Awake()
    {
        base.Awake();
        boss = GetComponent<Boss>();
        
    }
    
    
}
