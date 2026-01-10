using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ObjectAnim : MonoBehaviour
{
    [System.Serializable]
    public struct TweenAnimation
    {
        public bool usesPosition;
        public Vector3 targetPosition;
        public bool usesRotation;
        public Vector3 targetRotation;
        public Ease ease;
        public float duration;
        public UnityEvent onComplete;
    }

    public TweenAnimation[] tweens;
    public void PlayAnimation()
    {
        Sequence totalSequence = DOTween.Sequence();
        foreach (var tween in tweens)
        {
            
            Sequence seq = DOTween.Sequence();
            if (tween.usesPosition)
            {
                seq.Join(transform.DOMove(tween.targetPosition, tween.duration).SetEase(tween.ease));
            }
            if (tween.usesRotation)
            {
                seq.Join(transform.DORotate(tween.targetRotation, tween.duration).SetEase(tween.ease));
            }
            seq.OnComplete(() => tween.onComplete?.Invoke());
            totalSequence.Append(seq);
        }
        totalSequence.Play();
    }
    
    public void PlayTween(int index)
    {
        if (index < 0 || index >= tweens.Length) return;
        var tween = tweens[index];
        Sequence seq = DOTween.Sequence();
        if (tween.usesPosition)
        {
            seq.Join(transform.DOMove(tween.targetPosition, tween.duration).SetEase(tween.ease));
        }
        if (tween.usesRotation)
        {
            seq.Join(transform.DORotate(tween.targetRotation, tween.duration).SetEase(tween.ease));
        }
        seq.OnComplete(() => tween.onComplete?.Invoke());
        seq.Play();
    }
}
