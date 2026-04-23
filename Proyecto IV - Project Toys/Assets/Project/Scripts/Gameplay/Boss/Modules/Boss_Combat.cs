using UnityEngine;

public class Boss_Combat : Entity_Combat
{
    private Boss boss;
    
    public override void Awake()
    {
        base.Awake();
        boss = GetComponent<Boss>();
        
    }
    
    
}
