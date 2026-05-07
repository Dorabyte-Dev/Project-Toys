using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class RumbleManager : MonoBehaviour
{
    [System.Serializable]
    public struct RumbleSettings
    {
        public string Tag;
        public float LowFrequency;
        public float HighFrequency;
        public float Duration;
    }
    public static Gamepad pad;
    private static Tween rumbleTween;
    
    public static RumbleSettings[] _rumbleSettings;
    public RumbleSettings[] rumbleSettings;

    public void Start()
    {
        _rumbleSettings = rumbleSettings;
    }

    public static void RumblePulse(string tag)
    {
        RumbleSettings rumble = _rumbleSettings.FirstOrDefault(l => l.Tag == tag);
        if (rumble.Tag == string.Empty)
        {
            Debug.LogWarning("Rumble tag not found: " + tag);
            return;
        }
        RumblePulse(rumble.LowFrequency, rumble.HighFrequency, rumble.Duration);
    }
    public static void RumblePulse(float lowFrequency, float highFrequency, float duration)
    {
        pad = Gamepad.current;
        
        if (pad != null)
        {
            if (rumbleTween != null && rumbleTween.IsActive())
            {
                rumbleTween.Kill();
            }

            pad.SetMotorSpeeds(lowFrequency, highFrequency);

            rumbleTween = DOVirtual.DelayedCall(duration, StopRumble);
        }
    }

    public static void StopRumble()
    {
        if (pad != null)
        {
            pad.SetMotorSpeeds(0f, 0f);
        }
    }
    
    private void OnDisable()
    {
        StopRumble();
    }
}
