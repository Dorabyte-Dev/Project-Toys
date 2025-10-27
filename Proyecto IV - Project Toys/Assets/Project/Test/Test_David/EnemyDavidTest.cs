using UnityEngine;

public class EnemyDavidTest : MonoBehaviour
{
    public bool jugadorDetectado;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (jugadorDetectado)
        {
            Debug.Log("TE ENCONTRÉ");
        }
    }
}
