using DG.Tweening;
using TMPro;
using UnityEngine;

public class PruebaDañoEnemigo : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 initialPosition;
    public int damage;
    public TMP_Text damageText;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        //InvokeRepeating(nameof(RecieveDamage), 1, 1);
    }


    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RecieveDamage(damage);
        }
    }

    private void RecieveDamage(int damage)
    {
        damageText.text = damage.ToString();
        damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, 1);
        Jump();
    }

    public void Jump()
    {
        rectTransform.anchoredPosition = initialPosition;
        rectTransform.DOJumpAnchorPos(Vector2.right, 1, 1, .5f);
        damageText.DOFade(0, .5f);
    }
}
