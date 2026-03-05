using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [Tooltip("Si está activo, el evento sólo se lanzará una vez.")]
    public bool triggerOnce = true;
    
    private bool _hasBeenTriggered;

    public UnityEvent eventTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (triggerOnce && _hasBeenTriggered) return;
            eventTrigger?.Invoke();
            _hasBeenTriggered = true;
        }
    }
    
    public void Reset()
    {
        _hasBeenTriggered = false;
    }
}




