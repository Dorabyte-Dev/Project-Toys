using UnityEngine;

public class WallSlamVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem halfStartBurstParticles;
    [SerializeField] private ParticleSystem dustParticles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlayHalfStartBurst()
    {
        halfStartBurstParticles.Play();
    }
    
    public void PlayDust()
    {
        dustParticles.Play();
    }

    public void SetInactive()
    {
        gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
