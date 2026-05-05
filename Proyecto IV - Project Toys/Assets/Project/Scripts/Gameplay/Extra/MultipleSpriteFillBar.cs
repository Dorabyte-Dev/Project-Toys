using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MultipleSpriteFillBar : MonoBehaviour
{
    private float _value;

    public float Value
    {
        get => _value;
        set
        {
            _value = Mathf.Clamp01(value);
            UpdateFillBars();
        }
    }

    public Image[] fillBars;

    void UpdateFillBars()
    {
        float totalValue = Value * fillBars.Length;

        for (int i = 0; i < fillBars.Length; i++)
        {
            float targetFillAmount = Mathf.Clamp01(totalValue - i);
            float currentFillAmount = fillBars[i].fillAmount;

            if (currentFillAmount < 1f && targetFillAmount >= 1f)
            {
                OnBarFilled(i);
            }
            else if (currentFillAmount > 0f && targetFillAmount <= 0f)
            {
                OnBarEmpty(i);
            }
            else if (currentFillAmount >= 1f && targetFillAmount < 1f)
            {
                OnBarStartedEmptying(i);
            }
            else if (currentFillAmount <= 0f && targetFillAmount > 0f)
            {
                OnBarStartedFill(i);
            }

            fillBars[i].fillAmount = targetFillAmount;
        }
    }

    void OnBarFilled(int barIndex)
    {
        
    }

    void OnBarEmpty(int barIndex)
    {
        GameObject barParent = fillBars[barIndex].transform.parent.gameObject;
        barParent.SetActive(true);
        barParent.transform.DOMoveY(barParent.transform.position.y + UIManager.Instance.healthBlockAnimationDistance,
            UIManager.Instance.healthBlockAnimationDuration).SetEase(UIManager.Instance.healthBlockAnimationEase);
    }

    void OnBarStartedEmptying(int barIndex)
    {
        
    }

    void OnBarStartedFill(int barIndex)
    {
        GameObject barParent = fillBars[barIndex].transform.parent.gameObject;
        barParent.SetActive(true);
        barParent.transform.DOMoveY(barParent.transform.position.y - UIManager.Instance.healthBlockAnimationDistance,
            UIManager.Instance.healthBlockAnimationDuration).SetEase(UIManager.Instance.healthBlockAnimationEase);
        
        //Tween to lower the bar
    }
}
