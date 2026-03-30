using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueComment comment;

    [Tooltip("Is this already commented?")]
    public bool comentado = false;

    public void TriggerComment()
    {
        if (!comentado)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartComment(comment);
                comentado = true;
            }
            else
            {
                Debug.LogWarning("DialogueManager instance not found in the scene. Are you dumb of what?");
            }
        }
    }
}
