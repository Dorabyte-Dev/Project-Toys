using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private string musicName;
    [SerializeField] private bool crossfade = true;
    public void TriggerMusic()
    {
        if (crossfade)
        {
            SoundManager.instance.ChangeCurrentTheme(musicName);
        }
        else
        {
            if(SoundManager.instance.currentTheme != null)
            {
                SoundManager.instance.Stop(SoundManager.instance.currentTheme.name);
            }
            SoundManager.instance.Play(musicName);
        }
    }

    public void TriggerSFX()
    {
        SoundManager.instance.Play(musicName);
    }
}
