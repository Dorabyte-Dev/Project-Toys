using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public EnemySpawner spawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            spawner.EnemyDead(this.gameObject);
            Destroy(gameObject);
        }
    }
}
