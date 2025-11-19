using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyFactory : IObjectFactory
{

    private readonly Dictionary<GameObject, IObjectPool<GameObject>> _pools = new Dictionary<GameObject, IObjectPool<GameObject>>();
    public EnemyFactory()
    {
    }
    public GameObject Get(GameObject prefab, Vector3 position)
    {
        if (!_pools.ContainsKey(prefab))
            CreatePoolFor(prefab);

        GameObject obj = _pools[prefab].Get();

        //Do Some Logic

        return obj;
    }

    public void Dispose(GameObject obj)
    {
        //EnemyDead

        //_pools[original].Release(obj);
    }

    private void CreatePoolFor(GameObject prefab)
    {
        _pools[prefab] = new ObjectPool<GameObject>(
            createFunc: () => Create(prefab),
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyObject,
            defaultCapacity: 20,
            maxSize: 50
        );
    }
    //Pool Callbacks
    private GameObject Create(GameObject prefab)
    {
        var instance = Object.Instantiate(prefab);

        //Instance new object

        return instance;
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
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
