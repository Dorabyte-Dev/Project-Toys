using UnityEngine;

public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSystem input;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        // Asociamos el player al estado
        this.player = player;

        // Asociamos los componentes comunes
        anim = player.anim;
        rb = player.rb;

        // Asociamos el sistema de input del jugador
        this.input = player.input;
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();
        // Aqui va la logica del estado
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

}
