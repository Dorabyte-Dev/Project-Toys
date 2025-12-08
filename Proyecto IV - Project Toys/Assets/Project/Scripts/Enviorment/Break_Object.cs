using System.Collections.Generic;
using UnityEngine;

public class Break_Object : MonoBehaviour
{
    public string tagName = "Damage";

    [SerializeField] private List<Rigidbody> rigidbodies = new List<Rigidbody>();

    void Start()
    {
        // Recoge todos los Rigidbody hijos y los pone en la lista
        rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());

        // Inicialmente todos en isKinematic = true para que no se muevan
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    public void ActivateDestruction()
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        Destroy(gameObject, 3f);
    }
}