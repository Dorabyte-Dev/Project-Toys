using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

[System.Serializable]
public class CameraSwitch
{
    public CinemachineCamera cam;

    public void RaisePriority(int priority)
    {
        cam.Priority += priority;
    }

    public void LowerPriority(int priority)
    {
        cam.Priority -= priority;
    }
}
