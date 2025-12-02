using UnityEngine;
using UnityEngine.UI;

public class ComboBarUIManager : MonoBehaviour
{
    public static ComboBarUIManager Instance;
    [SerializeField] private Image comboBarLeft;
    [SerializeField] private Image comboBarRight;
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
}
