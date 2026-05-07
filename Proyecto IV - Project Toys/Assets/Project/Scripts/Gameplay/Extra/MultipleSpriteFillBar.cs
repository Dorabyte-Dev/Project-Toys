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
    private float blockYInitialDistance;

    private void Start()
    {
        blockYInitialDistance = fillBars[0].transform.position.y;
    }
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
        GameObject barParent = fillBars[barIndex].transform.parent.gameObject;
        barParent.transform.DOMoveY(blockYInitialDistance,
            UIManager.Instance.healthBlockAnimationDuration).SetEase(UIManager.Instance.healthBlockAnimationEase);
    }

    void OnBarEmpty(int barIndex)
    {
        GameObject barParent = fillBars[barIndex].transform.parent.gameObject;
        barParent.SetActive(true);
        barParent.transform.DOMoveY(blockYInitialDistance + UIManager.Instance.healthBlockAnimationDistance,
            UIManager.Instance.healthBlockAnimationDuration).SetEase(UIManager.Instance.healthBlockAnimationEase);
        
        Debug.LogWarning("Bar " + barIndex + " emptied");
        Debug.LogWarning("InitialPosition: " + blockYInitialDistance + " TargetPosition: " + (blockYInitialDistance + UIManager.Instance.healthBlockAnimationDistance));
    }

    void OnBarStartedEmptying(int barIndex)
    {
        
    }

    void OnBarStartedFill(int barIndex)
    {
        GameObject barParent = fillBars[barIndex].transform.parent.gameObject;
        barParent.SetActive(true);
        barParent.transform.DOMoveY(blockYInitialDistance,
            UIManager.Instance.healthBlockAnimationDuration).SetEase(UIManager.Instance.healthBlockAnimationEase);
        
        Debug.LogWarning("Bar " + barIndex + " emptied");
        Debug.LogWarning("InitialPosition: " + blockYInitialDistance + " TargetPosition: " + blockYInitialDistance);
    }
}
