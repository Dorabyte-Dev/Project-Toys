using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("Current UI Stats")] 
    private int currentDamageIndex;
    
    [Header("Enemy Canvas")]
    public GameObject canvasObj;
    private EnemyCanvas enemyCanvas;
    [Space(10)]

    [Header("Enemy Health")]
    private Enemy_Health enemyLifeScript;
    [Space(10)]

    [Header("Enemy Damage Number")]
    private RectTransform[] rectTransforms;
    private Vector2 initialPosition;
    private TMP_Text[] damagesText;
    [Space(10)]

    [Header("Config")]
    [SerializeField]private float damageNumberVelocity;

    private void Awake()
    {
        enemyCanvas = canvasObj.GetComponent<EnemyCanvas>();
        enemyLifeScript = GetComponentInParent<Enemy_Health>();
        if (enemyCanvas.fill == null)
        {
            enemyCanvas.fill = GetComponent<Image>();
        }
        rectTransforms = new RectTransform[enemyCanvas.damageNumbers.Length];
        damagesText = new TMP_Text[enemyCanvas.damageNumbers.Length];
    }

    void Start()
    {
        InitializeDamageNumber();
    }

    private void OnEnable()
    {
        InputDeviceManager.AlCambiarDispositivo += CheckDevice;
    }

    private void OnDisable()
    {
        InputDeviceManager.AlCambiarDispositivo -= CheckDevice;
    }

    private void InitializeDamageNumber()
    {
        for (int i = 0; i < enemyCanvas.damageNumbers.Length; i++)
        {
            rectTransforms[i] = enemyCanvas.damageNumbers[i].GetComponent<RectTransform>();
            damagesText[i] = enemyCanvas.damageNumbers[i].GetComponent<TMP_Text>();
            enemyCanvas.damageNumbers[i].SetActive(false);
        }
        initialPosition = rectTransforms[0].anchoredPosition;
        currentDamageIndex = 0;
    }

    void Update()
    {
        canvasObj.transform.rotation = Camera.main.transform.rotation;

        if (enemyLifeScript.currentHp >= enemyLifeScript.maxHp)
        {
            enemyCanvas.lifeBar.SetActive(false);
        }
        else
        {
            enemyCanvas.lifeBar.SetActive(true);
        }
        enemyCanvas.fill.fillAmount = enemyLifeScript.currentHp / enemyLifeScript.maxHp;
    }

    public void ReceiveDamage(int damage)
    {
        enemyCanvas.damageNumbers[currentDamageIndex].SetActive(true);
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
            enemyCanvas.damageNumbers[currentDamageIndex].SetActive(false);
        });
        currentDamageIndex++;
        if (currentDamageIndex >= enemyCanvas.damageNumbers.Length)
        {
            currentDamageIndex = 0;
        }
    }
    
    public void ShowExecutionUI()
    {
        if(enemyCanvas.executionAffordance == null) return;
        enemyCanvas.executionAffordance.SetActive(true);
    }
    
    public void HideExecutionUI()
    {
        if(enemyCanvas.executionAffordance == null) return;
        enemyCanvas.executionAffordance.SetActive(false);
    }

    private void CheckDevice(InputDeviceManager.Devices dispositivo)
    {
        if(enemyCanvas.executionAffordance == null) return;
        switch (dispositivo)
        {
            case InputDeviceManager.Devices.Teclado:
                enemyCanvas.keyboardAffordance.SetActive(true);
                enemyCanvas.controllerAffordance.SetActive(false);
                break;
            case InputDeviceManager.Devices.Mando:
                enemyCanvas.keyboardAffordance.SetActive(false);
                enemyCanvas.controllerAffordance.SetActive(true);
                break;
            default:
                enemyCanvas.keyboardAffordance.SetActive(false);
                enemyCanvas.controllerAffordance.SetActive(false);
                break;
        }
        
    }
    
    
}
