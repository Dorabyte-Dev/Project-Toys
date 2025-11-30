using UnityEngine;
using UnityEngine.UI;

public class ComboBarUIManager : MonoBehaviour
{
    [SerializeField] private Image comboBarLeft;
    [SerializeField] private Image comboBarRight;
    private int _fillAmount;
    public int FillAmount
    {
        get => _fillAmount;
        set 
        {
            _fillAmount = value;
            comboBarLeft.fillAmount = _fillAmount;
            comboBarRight.fillAmount = _fillAmount;
        }
    }
}
