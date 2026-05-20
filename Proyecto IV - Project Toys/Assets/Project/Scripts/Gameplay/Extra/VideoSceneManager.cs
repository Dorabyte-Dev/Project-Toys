using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneManager : MonoBehaviour
{
    public VideoPlayer video;
    public float delayTime = 2.0f;
    public UnityEvent OnVideoStart;
    public UnityEvent OnVideoEnd;
    void Start()
    {
        video = GetComponent<VideoPlayer>();
        video.loopPointReached += DelayedEvent;
        OnVideoStart?.Invoke();
    }

    private void Update()
    {

    }
    private void DelayedEvent(VideoPlayer video)
    {
        DOVirtual.DelayedCall(delayTime, () =>
        {
            OnVideoEnd?.Invoke();
        });
    }
}
