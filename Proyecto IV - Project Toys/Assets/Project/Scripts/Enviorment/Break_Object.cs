using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Break_Object : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    [SerializeField] private List<MeshCollider> colliders = new List<MeshCollider>();
    [SerializeField] private List<Transform> transforms = new List<Transform>();

    void Start()
    {
        // Get all rigibodys and mesh colliders of child objects
        rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        colliders.AddRange(GetComponentsInChildren<MeshCollider>());
        transforms.AddRange(GetComponentsInChildren<Transform>());

        // Set the parameters to the child objects
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }

        foreach (var col in colliders)
        {
            col.enabled = false;
        }
    }

    public void ActivateDestruction()
    {
        // Activate the destruction of the Game Object
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (var col in colliders)
        {
            col.enabled = true;
        }   
        
        Invoke("ShrinkEffect", 0.8f);
    }

    private void ShrinkEffect()
    {
        float delayBeforePop = 0.5f;
        float popDuration = 0.8f;

        foreach (var t in transforms)
        {
            Sequence popSequence = DOTween.Sequence();

            popSequence.AppendInterval(delayBeforePop);
            
            popSequence.Append(t.DOScale(t.localScale * .8f, popDuration * 0.01f));
            popSequence.Append(t.DOScale(Vector3.zero, popDuration * 0.8f).SetEase(Ease.InBack));
            popSequence.OnComplete(() => Destroy(gameObject));
        }
    }
}