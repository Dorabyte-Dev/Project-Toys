using UnityEngine;

public class ZoneDoor : MonoBehaviour
{
    //By Animator in the future

    //public bool closeState;
    public bool startClosed;
    public GameObject Door;
    private Animator _anim;
    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.enabled = false;
        gameObject.SetActive(startClosed);
    }

    void Update()
    {
        
    }

    public void Open()
    {
        if (_anim != null)
        {
            _anim.SetTrigger("Open");
            SoundManager.instance.Play("LimitBlocksFalling(Test)", 2f);
        }
    }
    public void Close()
    {
        _anim.enabled = true;
        SoundManager.instance.Play("LimitBlocksFalling(Test)", 1f);
    }
}
