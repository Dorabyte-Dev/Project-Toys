using DG.Tweening;
using UnityEngine;

public class PruebaDañoEnemigo : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 initialPosition;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        InvokeRepeating(nameof(Jump), 1, 1);
    }


    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void Jump()
    {
        rectTransform.anchoredPosition = initialPosition;
        rectTransform.DOJumpAnchorPos(Vector2.right, 1, 1, .5f);
    }
}
