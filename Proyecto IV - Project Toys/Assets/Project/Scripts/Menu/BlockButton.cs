using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BlockButton : MonoBehaviour
{
    public System.Action onClick;
    public UnityEvent DestroyBlock;
    public ParticleSystem hitParticles;  // Tus part�culas
    public GameObject brokenPrefab;      // El bloque roto
    public float shakeIntensity = 0.05f;
    public int hitsToBreak = 3;
    public Renderer renderer;

    private int currentHits = 0;

    private void Awake()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();
    }
    private void Start()
    {
        
    }

    private void OnMouseDown()
    {
        BlockHit();
    }

    public void BlockHit()
    {
        currentHits++;

        // Vibración
        Shake();

        // Part�culas
        /*if (hitParticles != null)
            hitParticles.Play();*/

        // Si se ha roto
        if (currentHits >= hitsToBreak)
        {
            BreakBlock();
        }
    }

    private void Shake()
    {
        transform.DOShakePosition(0.1f, shakeIntensity)
            .SetEase(Ease.OutQuad);
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
