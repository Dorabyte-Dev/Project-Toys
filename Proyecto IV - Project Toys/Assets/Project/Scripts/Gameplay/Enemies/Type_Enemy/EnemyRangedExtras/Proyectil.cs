using UnityEngine;

public class Proyectil : MonoBehaviour
{
    private bool isReleased = false;
    
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float speed;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Release()
    {
        isReleased = true;
    }
}
