using UnityEngine;

public class Player_AnimationTriggers : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void CurrentStateTrigger()
    {
        player.CallAnimationTrigger();
    }
}
