using UnityEngine;
using System.Collections;

public class BlockButton : MonoBehaviour
{
    public System.Action onClick;

    public ParticleSystem hitParticles;  // Tus partículas
    public GameObject brokenPrefab;      // El bloque roto
    public float shakeIntensity = 0.05f;
    public int hitsToBreak = 3;

    private int currentHits = 0;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    private void OnMouseDown()
    {
        currentHits++;

        // Vibración
        StartCoroutine(Shake());

        // Partículas
        if (hitParticles != null)
            hitParticles.Play();

        // Si se ha roto
        if (currentHits >= hitsToBreak)
        {
            BreakBlock();
        }
        else
        {
            onClick?.Invoke();
        }
    }

    private IEnumerator Shake()
    {
        float time = 0.1f;
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeIntensity;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

    private void BreakBlock()
    {
        Instantiate(brokenPrefab, transform.position, transform.rotation);
        Destroy(gameObject);  // Destruye el bloque original
    }
}
