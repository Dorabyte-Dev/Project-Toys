using UnityEngine;

public abstract class EntityState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody rb;

    public EntityState(Player player, StateMachine stateMachine, string stateName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = stateName;

        this.anim = player.anim;
        this.rb = player.rb;
    }

    public virtual void Enter()
    {
        // Cada vez que se cambie de estado se llamará a este método
        anim.SetBool(animBoolName, true);

    }

    public virtual void Update()
    {
        // Aqui va la logica del estado
        Debug.Log("Updating state: " + animBoolName);
    }

    public virtual void Exit()
    {
        // Esto se llamara cada vez que salgamos de un estado y entremos en otro
        anim.SetBool(animBoolName, false);
    }

}
