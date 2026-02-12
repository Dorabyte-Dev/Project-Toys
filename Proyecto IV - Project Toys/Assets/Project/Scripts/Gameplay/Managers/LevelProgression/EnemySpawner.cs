using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    [SerializeField] private Collider spawnArea;
    public float spawnDelay;
    private int currentWave;
    [FormerlySerializedAs("spawnCount")] public int[] wavesSpawnCount;
    private int enemiesDead;
    [SerializeField]private List<GameObject> enemiesSpawned = new List<GameObject>();
    public UnityEvent endCombat;
    private static IObjectFactory enemyFactory;
    private CameraManager camManager;
    private CameraSwitch cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //cam = GetComponent<CameraSwitch>();
        camManager = FindAnyObjectByType<CameraManager>();
        if(enemyFactory == null )
        {
            enemyFactory = new EnemyFactory();
        }
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR 
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCombat();
        }
        #endif
    }

    public void StartCombat()
    {
        currentWave = 0;
        StartCoroutine(SpawnEnemies());
        if(cam != null && camManager != null)
        {
            camManager.ToggleOnCombatCamera(cam);
        }
        else
        {
            Debug.LogWarning("Camera or Camera Manager not assigned!");
        }
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < wavesSpawnCount[currentWave]; i++)
        {
            //Codigo de la generacion aleatoria dentro de bounds
            Vector3 newPosition = GetRandomNavMeshPoint(spawnArea);


            GameObject newEnemy = Instantiate(enemy, newPosition, Quaternion.identity);
            //GameObject newEnemy = enemyFactory.Get(enemy, newPosition);
            Enemy newEnemyScript = newEnemy.GetComponentInChildren<Enemy>();
            
            //Instanciamos el WaveManager para registrarlo en la lista de enemigos
            EnemyWaveManager.Instance?.RegisterEnemy(newEnemyScript);
            enemiesSpawned.Add(newEnemyScript.gameObject);
            newEnemyScript.spawner = this;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void EnemyDead(GameObject enemy)
    {
        if (!enemiesSpawned.Contains(enemy)) return;

        enemiesSpawned.Remove(enemy);
        //enemyFactory.Dispose(enemy);
        //enemy.transform.parent.gameObject.SetActive(false);
        enemiesDead++;
        if(wavesSpawnCount[currentWave] == enemiesDead)
        {
            if(currentWave + 1 < wavesSpawnCount.Length)
            {
                currentWave++;
                enemiesDead = 0;
                StartCoroutine(SpawnEnemies());
            }
            else
            {
                EndCombat();
            }
        }
    }

    private void EndCombat()
    {
        if (cam != null && camManager != null)
        {
            camManager.UnToggleOnCombatCamera();
        }
        else
        {
            Debug.LogWarning("Camera or Camera Manager not assigned!");
        }

        endCombat.Invoke();
    }


    public void ResetCombat()
    {
        foreach (GameObject enemy in enemiesSpawned)
        {
            Destroy(enemy);
            
        }
        enemiesDead = 0;
        enemiesSpawned.Clear();
    }
    Vector3 GetRandomNavMeshPoint(Collider col)
    {
        Bounds bounds = col.bounds;
        Vector3 randomPoint;

        NavMeshHit hit;
        do
        {
            randomPoint.x = Random.Range(bounds.min.x, bounds.max.x);
            randomPoint.y = Random.Range(bounds.min.y, bounds.max.y);
            randomPoint.z = Random.Range(bounds.min.z, bounds.max.z);
        }
        while (!NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas));

        return hit.position;
    }
}
