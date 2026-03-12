using UnityEngine;

public abstract class EntityState
{
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;

    protected float stateTimer;

    protected bool triggerCalled;

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        // Asociamos la maquina de estados al estado
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        // Cada vez que se cambie de estado se llamar� a este m�todo
        anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        // Aqui va la logica del estado
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        // Esto se llamara cada vez que salgamos de un estado y entremos en otro
        anim.SetBool(animBoolName, false);
    }

    public void CallAnimationTrigger()
    {
        triggerCalled = true;
    }
}
