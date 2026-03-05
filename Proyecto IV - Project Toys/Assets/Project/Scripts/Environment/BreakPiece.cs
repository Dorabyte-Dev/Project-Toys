using UnityEngine;

public class BreakPiece : MonoBehaviour
{
    [SerializeField] private Break_Object parent;
    private void Awake()
    {
        parent = GetComponentInParent<Break_Object>();
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(parent.tagName))
        {
            if (parent != null)
            {
                parent.ActivateDestruction();
            }
        }
    }*/
}