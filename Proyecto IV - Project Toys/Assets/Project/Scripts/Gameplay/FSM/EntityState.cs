using UnityEngine;

public abstract class EntityState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string stateName;

    public EntityState(Player player, StateMachine stateMachine, string stateName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter()
    {
        // Cada vez que se cambie de estado se llamará a este método
        Debug.Log("Entering state: " + stateName);

    }

    public virtual void Update()
    {
        // Aqui va la logica del estado
        Debug.Log("Updating state: " + stateName);
    }

    public virtual void Exit()
    {
        // Esto se llamara cada vez que salgamos de un estado y entremos en otro
        Debug.Log("Exiting state: " + stateName);
    }

}
