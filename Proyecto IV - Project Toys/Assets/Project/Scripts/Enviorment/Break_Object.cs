using System.Collections.Generic;
using UnityEngine;

public class Break_Object : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    [SerializeField] private List<MeshCollider> colliders = new List<MeshCollider>();

    void Start()
    {
        // Get all rigibodys and mesh colliders of child objects
        rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        colliders.AddRange(GetComponentsInChildren<MeshCollider>());

        // Set the parameters to the child objects
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }

        foreach (var col in colliders)
        {
            col.enabled = false;
        }
    }

    public void ActivateDestruction()
    {
        // Activate the destruction of the Game Object
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (var col in colliders)
        {
            col.enabled = true;
        }

        Destroy(gameObject, 3f);
    }
}