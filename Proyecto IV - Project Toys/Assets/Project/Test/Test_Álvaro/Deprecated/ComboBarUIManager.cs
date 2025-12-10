using UnityEngine;
using UnityEngine.UI;

public class ComboBarUIManager : MonoBehaviour
{
    public static ComboBarUIManager Instance;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
