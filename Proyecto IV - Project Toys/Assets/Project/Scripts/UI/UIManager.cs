using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField]private Player player;
    [SerializeField] private Image healthBar;
    private float _healthBarFillAmount;
    public float HealthBarFillAmount
    {
        get => _healthBarFillAmount;
        set 
        {
            _healthBarFillAmount = Mathf.Clamp01(value);
            healthBar.fillAmount = _healthBarFillAmount;
        }
    }
    
    [SerializeField] private Image comboBarLeft;
    [SerializeField] private Image comboBarRight;

    [SerializeField] private RawImage curtain;
    private static RawImage curtainInstance;
    [HideInInspector]public bool IsComboBarFull => ComboBarFillAmount >= 1f;
    private float _comboBarFillAmount;
    public float ComboBarFillAmount
    {
        get => _comboBarFillAmount;
        set 
        {
            _comboBarFillAmount = Mathf.Clamp01(value);
            comboBarLeft.fillAmount = _comboBarFillAmount;
            comboBarRight.fillAmount = _comboBarFillAmount;

            if(Mathf.Approximately(_comboBarFillAmount, 1))
            {
                comboBarLeft.color = Color.blue;
                comboBarRight.color = Color.blue;
            }
            else
            {
                comboBarLeft.color = Color.white;
                comboBarRight.color = Color.white;
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }
        curtainInstance = curtain;
        curtainInstance.material.SetFloat("_MaskScale", 1f);
    }

    private void Update()
    {
        HealthBarFillAmount = player.GetCurrentHealth() / player.GetMaxHealth();
    }
    
    public static void CloseCurtain(Action onComplete = null)
    {
        curtainInstance.material.DOFloat(0,"_MaskScale", 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
    
    public static void OpenCurtain(float delay = 0, Action onComplete = null)
    {
        
        curtainInstance.material.DOFloat(1,"_MaskScale", 1f).SetEase(Ease.InOutQuad).SetDelay(delay).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void OnDisable()
    {
        curtainInstance.material.SetFloat("_MaskScale", 1f);
    }
}
