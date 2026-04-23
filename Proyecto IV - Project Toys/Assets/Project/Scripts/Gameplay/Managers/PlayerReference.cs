using UnityEngine;

public static class PlayerReference
{
    public static Transform playerTransform;
    
    public static void RegisterPlayer(Transform player)
    {
        playerTransform = player;
    }
}
