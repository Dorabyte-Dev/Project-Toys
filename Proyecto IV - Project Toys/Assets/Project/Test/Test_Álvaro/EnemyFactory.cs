using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.Splines;
using static UnityEngine.UI.Image;

public class EnemyFactory : IObjectFactory
{

    private readonly Dictionary<GameObject, IObjectPool<GameObject>> _pools = new Dictionary<GameObject, IObjectPool<GameObject>>();
    public EnemyFactory()
    {
    }
    public GameObject Get(GameObject prefab, Vector3 position)
    {
        if (!_pools.ContainsKey(prefab))
        {
            CreatePoolFor(prefab, position);
            Debug.LogWarning("Created new pool for " + prefab.name);
        }
            

        GameObject obj = _pools[prefab].Get();
        obj.GetComponentInChildren<NavMeshAgent>().Warp(position);
        //Do Some Logic

        return obj;
    }

    public void Dispose(GameObject obj)
    {
        //EnemyDead
        Enemy enemy = obj.GetComponent<Enemy>();
        if(enemy != null && enemy.originalPrefab != null) 
        {
            _pools[enemy.originalPrefab].Release(obj.transform.parent.gameObject);
        }
        else
        {
            Debug.LogError("Error in dispose method. Check if originalPrefab is not assigned or if a foreign not-enemy class has been fed to the factory.");
        }
        
    }

    private void CreatePoolFor(GameObject prefab, Vector3 position)
    {
        _pools[prefab] = new ObjectPool<GameObject>(
            createFunc: () => Create(prefab, position),
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyObject,
            defaultCapacity: 20,
            maxSize: 50
        );

        Debug.Log("New Pool created");
    }
    //Pool Callbacks
    private GameObject Create(GameObject prefab, Vector3 position)
    {
        GameObject newEnemy = Object.Instantiate(prefab, position, Quaternion.identity);
        newEnemy.GetComponentInChildren<Enemy>().originalPrefab = prefab;

        //Instance new object

        return newEnemy;
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
        
        Debug.Log("Enemy reused");
    }

    private void OnRelease(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyObject(GameObject obj)
    {
        Object.Destroy(obj);
    }
}
