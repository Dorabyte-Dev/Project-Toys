using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class BlockButton : MonoBehaviour
{
    public System.Action onClick;
    public UnityEvent DestroyBlock;
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
        Vector3 startPos = transform.position;  // posición actual real
        float time = 0.1f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.position = startPos + Random.insideUnitSphere * shakeIntensity;
            yield return null;
        }

        transform.position = startPos;
    }

    private void BreakBlock()
    {
        GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);

        // Copiar escala del bloque original
        broken.transform.localScale = transform.localScale;

        DestroyBlock?.Invoke();
        //Fader.Instance.FadeToScene("Game");

        Destroy(gameObject);
        
    }
}
