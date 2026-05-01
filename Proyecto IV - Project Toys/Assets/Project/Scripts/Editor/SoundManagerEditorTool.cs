using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class SoundManagerEditorTool
{
    [MenuItem("Dorabyte/Add Sounds Batch To SoundManager", false, 20)]
    public static void AddSelectedClipsToSoundManager()
    {
        AudioClip[] selectedClips = Selection.GetFiltered<AudioClip>(SelectionMode.DeepAssets);
        
        if (selectedClips.Length == 0)
        {
            Debug.LogWarning("No AudioClips selected.");
            return;
        }

        SoundManager soundManager = Object.FindFirstObjectByType<SoundManager>();
        
        if (soundManager == null)
        {
            Debug.LogError("SoundManager not found in the scene. Please add a SoundManager to your scene before using this tool.");
            return;
        }

        Undo.RecordObject(soundManager, "Add Sounds Batch To SoundManager");

        List<Sound> currentSounds = soundManager.sounds != null ? soundManager.sounds.ToList() : new List<Sound>();
        int addedCount = 0;
        foreach (AudioClip clip in selectedClips)
        {
            if (currentSounds.Any(s => s.clip == clip))
            {
                Debug.Log($"Clip '{clip.name}' already in SoundManager, skipping.");
                continue;
            }

            string soundName = clip.name.Replace("SFX_", "").Replace("Music_", "");
            Sound newSound = new Sound
            {
                name = soundName,           
                clip = clip,
                volume = 1f,                
                type = Sound.SoundType.sfx  
            };

            currentSounds.Add(newSound);
            addedCount++;
        }

        soundManager.sounds = currentSounds.ToArray();
        
        EditorUtility.SetDirty(soundManager);

        Debug.Log($"Added {addedCount} new sounds to SoundManager: {string.Join(", ", currentSounds.Select(s => s.name))}");
        currentSounds.Clear();
    }

    [MenuItem("Dorabyte/Add Sounds Batch To SoundManager", true)]
    public static bool ValidateAddSelectedClips()
    {
        return Selection.GetFiltered<AudioClip>(SelectionMode.DeepAssets).Length > 0;
    }
}
