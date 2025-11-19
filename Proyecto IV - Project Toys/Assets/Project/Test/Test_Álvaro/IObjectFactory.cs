using UnityEngine;

public interface IObjectFactory
{
    public GameObject Get(GameObject obj, Vector3 position);
    public void Dispose(GameObject obj);
}
