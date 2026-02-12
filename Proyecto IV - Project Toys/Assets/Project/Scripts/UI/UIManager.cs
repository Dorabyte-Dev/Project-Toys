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

    private void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }
    }

    private void Update()
    {
        HealthBarFillAmount = player.GetCurrentHealth() / player.GetMaxHealth();
    }
}
