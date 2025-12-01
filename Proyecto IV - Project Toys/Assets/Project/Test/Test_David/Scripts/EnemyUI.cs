using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("Current UI Stats")] 
    [SerializeField] private int currentDamageIndex;
    
    [Header("Enemy Canvas")]
    public GameObject canvas;
    [Space(10)]

    [Header("Enemy Health")]
    public Image fill;
    public GameObject lifeBar;
    private Enemy_Health enemyLifeScript;
    [Space(10)]

    [Header("Enemy Damage Number")]
    public GameObject[] damageNumbers;
    private RectTransform[] rectTransforms;
    private Vector2 initialPosition;
    private TMP_Text[] damagesText;
    [Space(10)]

    [Header("Config")]
    [SerializeField]private float damageNumberVelocity;

    private void Awake()
    {
        enemyLifeScript = GetComponentInParent<Enemy_Health>();
        if (fill == null)
        {
            fill = GetComponent<Image>();
        }
        rectTransforms = new RectTransform[damageNumbers.Length];
        damagesText = new TMP_Text[damageNumbers.Length];
    }

    void Start()
    {
        InitializeDamageNumber();
    }

    private void InitializeDamageNumber()
    {
        for (int i = 0; i < damageNumbers.Length; i++)
        {
            rectTransforms[i] = damageNumbers[i].GetComponent<RectTransform>();
            damagesText[i] = damageNumbers[i].GetComponent<TMP_Text>();
            damageNumbers[i].SetActive(false);
        }
        initialPosition = rectTransforms[0].anchoredPosition;
        currentDamageIndex = 0;
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

    public void ReceiveDamage(int damage)
    {
        damageNumbers[currentDamageIndex].SetActive(true);
        damagesText[currentDamageIndex].text = damage.ToString();
        damagesText[currentDamageIndex].color = new Color(damagesText[currentDamageIndex].color.r, damagesText[currentDamageIndex].color.g, damagesText[currentDamageIndex].color.b, 1);
        DamageNumberAnimation();
    }

    private void DamageNumberAnimation()
    {
        rectTransforms[currentDamageIndex].anchoredPosition = initialPosition;
        rectTransforms[currentDamageIndex].DOJumpAnchorPos(Vector2.right, 1, 1, damageNumberVelocity);
        damagesText[currentDamageIndex].DOFade(0, damageNumberVelocity).OnComplete(() =>
        {
            damageNumbers[currentDamageIndex].SetActive(false);
        });
        currentDamageIndex++;
        if (currentDamageIndex >= damageNumbers.Length)
        {
            currentDamageIndex = 0;
        }
    }
}
