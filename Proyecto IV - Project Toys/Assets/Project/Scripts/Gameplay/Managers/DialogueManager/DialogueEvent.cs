using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueEvent
{
    public UnityEvent uEvent;

    public float timeOffset;

    public enum PlayWhen
    {
        PlayAtStart,
        PlayAtEnd,
        PlayAfterInput
    }

    public PlayWhen WhenToPlay;
}
