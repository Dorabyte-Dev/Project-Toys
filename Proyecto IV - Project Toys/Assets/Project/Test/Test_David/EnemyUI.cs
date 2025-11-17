using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("Enemy Canvas")]
    public GameObject canvas;
    [Space(10)]

    [Header("Enemy Health")]
    public Image fill;
    public GameObject lifeBar;
    private Enemy_Health enemyLifeScript;
    [Space(10)]

    [Header("Enemy Damage Number")]
    public GameObject damageNumber;
    private RectTransform rectTransform;
    private Vector2 initialPosition;
    private TMP_Text damageText;

    private void Awake()
    {
        enemyLifeScript = GetComponentInParent<Enemy_Health>();
        if (fill == null)
        {
            fill = GetComponent<Image>();
        }
        rectTransform = damageNumber.GetComponent<RectTransform>();
        damageText = damageNumber.GetComponent<TMP_Text>();
    }

    void Start()
    {
        initialPosition = rectTransform.anchoredPosition;
        damageNumber.SetActive(false);
    }

    void Update()
    {
        canvas.transform.rotation = Camera.main.transform.rotation;

        if (enemyLifeScript.currentHp >= enemyLifeScript.maxHp)
        {
            lifeBar.SetActive(false);
        }
        else
        {
            lifeBar.SetActive(true);
        }
        fill.fillAmount = enemyLifeScript.currentHp / enemyLifeScript.maxHp;
    }

    public void RecieveDamage(int damage)
    {
        damageNumber.SetActive(true);
        damageText.text = damage.ToString();
        damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, 1);
        DamageNumberAnimation();
    }

    private void DamageNumberAnimation()
    {
        rectTransform.anchoredPosition = initialPosition;
        rectTransform.DOJumpAnchorPos(Vector2.right, 1, 1, .5f);
        damageText.DOFade(0, .5f).OnComplete(() => damageNumber.SetActive(false));
    }
}
