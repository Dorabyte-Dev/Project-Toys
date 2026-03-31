using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public PlayableAsset timelineToPlay;

    public bool playOnlyOnce = true;
    private bool hasPlayed = false;

    [Header("Specific Events")]
    public UnityEvent OnCutsceneStarted;
    public UnityEvent OnCutsceneEnded;

    public void TriggerCutscene()
    {
        if (playOnlyOnce && hasPlayed) return;

        if (CutsceneManager.Instance != null && timelineToPlay != null)
        {
            CutsceneManager.Instance.OnAnyCutsceneEnd.AddListener(HandleCutsceneEnd);
            CutsceneManager.Instance.PlayCutscene(timelineToPlay);
            
            OnCutsceneStarted?.Invoke();
            
            hasPlayed = true;
        }
        else
        {
            Debug.LogWarning("No timeline assigned or CutsceneManager instance not found on " + gameObject.name);
        }
    }

    private void HandleCutsceneEnd()
    {
        CutsceneManager.Instance.OnAnyCutsceneEnd.RemoveListener(HandleCutsceneEnd);
        
        OnCutsceneEnded?.Invoke();
    }
}
