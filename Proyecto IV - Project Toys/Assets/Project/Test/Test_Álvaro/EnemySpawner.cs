using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public float maxSpawnDistance;
    public float spawnDelay;
    public int spawnCount;
    private List<GameObject> enemiesSpawned = new List<GameObject>();
    public UnityEvent endCombat;

    private CameraCollider cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<CameraCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCombat()
    {
        StartCoroutine(SpawnEnemies());
        if(cam != null)
        {
            cam.RaisePriority();
        }
    }

    public IEnumerator SpawnEnemies()
    {
        for(int i = 0; i < spawnCount; i++)
        {
            Vector3 newPosition = transform.position + new Vector3 (Random.Range(0, maxSpawnDistance), 0, Random.Range(0, maxSpawnDistance));
            GameObject newEnemy = Instantiate(enemy, newPosition, Quaternion.identity);
            enemiesSpawned.Add(newEnemy);
            newEnemy.GetComponent<EnemyTest>().spawner = this;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void EnemyDead(GameObject enemy)
    {
        if (!enemiesSpawned.Contains(enemy)) return;

        enemiesSpawned.Remove(enemy);

        if(enemiesSpawned.Count == 0)
        {
            if (cam != null)
            {
                cam.RaisePriority();
            }

            endCombat.Invoke();
        }
    }

}
