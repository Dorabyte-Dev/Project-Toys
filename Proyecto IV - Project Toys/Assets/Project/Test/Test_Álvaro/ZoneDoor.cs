using UnityEngine;

public class ZoneDoor : MonoBehaviour
{
    //By Animator in the future

    //public bool closeState;
    public bool startClosed;
    public GameObject Door;
    void Start()
    {
        Door.SetActive(startClosed);
    }

    void Update()
    {
        
    }

    public void Open()
    {
        Door.SetActive(false);
    }
    public void Close()
    {
        Door.SetActive(true);
    }
}
