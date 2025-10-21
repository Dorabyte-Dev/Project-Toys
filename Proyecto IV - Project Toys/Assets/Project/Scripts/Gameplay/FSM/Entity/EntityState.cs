using UnityEngine;

public abstract class EntityState
{
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody rb;

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        // Asociamos la maquina de estados al estado
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        // Cada vez que se cambie de estado se llamará a este método
        anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        // Aqui va la logica del estado
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public virtual void Exit()
    {
        // Esto se llamara cada vez que salgamos de un estado y entremos en otro
        anim.SetBool(animBoolName, false);
    }

}
