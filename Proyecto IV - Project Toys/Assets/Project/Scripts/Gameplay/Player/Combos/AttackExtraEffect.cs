using System;

[Flags]
public enum AttackExtraEffect
{
    Nothing = 0,
    CameraShake = 1 << 0,
    SlamEffect = 1 << 1,
    ControllerShake = 1 << 2
}