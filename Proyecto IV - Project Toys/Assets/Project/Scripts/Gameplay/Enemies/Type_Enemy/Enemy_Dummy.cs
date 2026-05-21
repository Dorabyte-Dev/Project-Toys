using UnityEngine;

public class Enemy_Dummy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        stateMachine.SwitchOffStateMachine();
    }

    protected override void Update()
    {
        
    }

    protected override void OnEnable()
    {
        
    }
}
