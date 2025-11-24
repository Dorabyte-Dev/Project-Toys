using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;

    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
}
