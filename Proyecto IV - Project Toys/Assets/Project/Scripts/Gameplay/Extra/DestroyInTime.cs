using System.Collections;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    public float timeToDestroy = 1f;
    void Start()
    {
        StartCoroutine(DestroyInTimeCouroutine(timeToDestroy));
    }

    private IEnumerator DestroyInTimeCouroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
