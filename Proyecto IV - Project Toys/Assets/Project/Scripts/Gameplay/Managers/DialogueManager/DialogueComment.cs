using UnityEngine;

[System.Serializable]
public class Sentence
{
    [Tooltip("Character speaking this sentence")]
    public CharacterProfile character;
    
    [Tooltip("UI Image to show in UI")]
    public CharacterEmotion emotion;
    
    [TextArea(3, 10)]
    public string sentenceText;

    [Header("Events")]
    public DialogueEvent sentenceEvent;
}

[System.Serializable]
public class DialogueComment
{
    public Sentence[] sentences;
}
