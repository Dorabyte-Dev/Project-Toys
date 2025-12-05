using UnityEngine;

public class BlockButton : MonoBehaviour
{
    public System.Action onClick;  // Lo que hará este bloque cuando se pulse

    private void OnMouseDown()
    {
        onClick?.Invoke();
    }
}
