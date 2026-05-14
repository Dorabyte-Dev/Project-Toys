using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnData
    {
        public GameObject enemyPrefab;
        public int enemyCount;
    }
    [System.Serializable]
    public struct Wave
    {
        public EnemySpawnData[] enemiesToSpawn;
        public int EnemyCount
        {
            get
            {
                int count = 0;
                foreach (EnemySpawnData data in enemiesToSpawn)
                {
                    count += data.enemyCount;
                }
                return count;
            }
        }
        public List<GameObject> TotalEnemies
        {
            get
            {
                List<GameObject> totalEnemies = new List<GameObject>();
                foreach (EnemySpawnData data in enemiesToSpawn)
                {
                    for (int i = 0; i < data.enemyCount; i++)
                    {
                        totalEnemies.Add(data.enemyPrefab);
                    }
                }
                return totalEnemies;
            }
        }
    }
    public GameObject enemy;
    [SerializeField] private Collider spawnArea;
    public float spawnDelay;
    private int currentWave;
    public Wave[] waves;
    private int enemiesDead;
    [SerializeField]private List<GameObject> enemiesSpawned = new List<GameObject>();
    public UnityEvent endCombat;
    private static IObjectFactory enemyFactory;
    private CameraManager camManager;
    private CameraGroup cam;

    [SerializeField] private ParticleSystem[] confettis;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //cam = GetComponent<CameraSwitch>();
        camManager = FindAnyObjectByType<CameraManager>();
        if(enemyFactory == null)
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
        if(camManager != null)
        {
            camManager.ToggleOnCombatCamera();
            if (cam != null)
            {
                camManager.SwitchCameraGroup(cam);
            }
        }
        else
        {
            Debug.LogWarning("Camera or Camera Manager not assigned!");
        }
    }

    private IEnumerator SpawnEnemies()
    {
        List<GameObject> enemiesToSpawn = waves[currentWave].TotalEnemies;
        //for (int i = 0; i < enemiesToSpawn.Count; i++)
        while(enemiesToSpawn.Count > 0)
        {
            //Codigo de la generacion aleatoria dentro de bounds
            Vector3 newPosition = GetRandomNavMeshPoint(spawnArea);
            int randomIndex = Random.Range(0, enemiesToSpawn.Count);
            GameObject newEnemy = Instantiate(enemiesToSpawn[randomIndex], newPosition, Quaternion.identity);
            enemiesToSpawn.RemoveAt(randomIndex);
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
        if(waves[currentWave].EnemyCount == enemiesDead)
        {
            if(currentWave + 1 < waves.Length)
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
        if (camManager != null)
        {
            camManager.UnToggleOnCombatCamera();
        }
        else
        {
            Debug.LogWarning("Camera Manager not assigned!");
        }
        
        if(confettis != null && confettis.Length > 0)
        {
            foreach (ParticleSystem confetti in confettis)
            {
                confetti.Play();
            }
        }
        
        FindFirstObjectByType<Player>().RevokeControl();
        
        DOVirtual.DelayedCall(3f,() => {
            endCombat.Invoke();
        });

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
