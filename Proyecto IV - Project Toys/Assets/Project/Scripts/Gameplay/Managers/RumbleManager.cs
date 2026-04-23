using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class RumbleManager : MonoBehaviour
{
    public static Gamepad pad;
    private static Tween rumbleTween;

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
