using System.Collections.Generic;
using UnityEngine;

public enum CharacterEmotion
{
    Neutral,
    Happy,
    Sad,
    Angry,
    Surprised,
    Scared,
    Thinking
}
[System.Serializable]
public struct EmotionPortrait
{
    public CharacterEmotion emotion;
    public Sprite portrait;
}

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue System/Character Profile")]
public class CharacterProfile : ScriptableObject
{
    public string characterName;
    public Color textColor = Color.white;
    
    [Tooltip("Sound Clip Name in SoundManager")]
    public string voiceBlipName; 

    [Tooltip("Character portraits for different emotions. Make sure to assign the correct emotion to each portrait.")]
    public List<EmotionPortrait> portraits;

    public Sprite GetPortrait(CharacterEmotion emotion)
    {
        foreach (var ep in portraits)
        {
            if (ep.emotion == emotion)
                return ep.portrait;
        }
        return null;
    }
}
