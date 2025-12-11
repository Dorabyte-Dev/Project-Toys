using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField]private Entity_Health playerHealth;
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
    [HideInInspector]public bool IsComboBarFull => FillAmount >= 1f;
    private float _fillAmount;
    public float FillAmount
    {
        get => _fillAmount;
        set 
        {
            _fillAmount = Mathf.Clamp01(value);
            comboBarLeft.fillAmount = _fillAmount;
            comboBarRight.fillAmount = _fillAmount;

            if(_fillAmount == 1)
            {
                comboBarLeft.color = Color.green;
                comboBarRight.color = Color.green;
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

    private void Update()
    {
        HealthBarFillAmount = playerHealth.currentHp / playerHealth.maxHp;
    }
}
