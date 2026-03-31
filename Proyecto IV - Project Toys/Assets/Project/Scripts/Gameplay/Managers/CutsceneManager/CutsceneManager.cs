using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using DG.Tweening;
using Unity.Cinemachine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [Header("Core Components")]
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private CinemachineCamera cutsceneCamera;

    [Header("Cinematic UI")]
    [SerializeField] private GameObject upperBand;
    [SerializeField] private GameObject lowerBand;

    [Header("Global Events")]
    [Tooltip("Triggers when ANY cutscene starts")]
    public UnityEvent OnAnyCutsceneStart;
    [Tooltip("Triggers when ANY cutscene ends")]
    public UnityEvent OnAnyCutsceneEnd;

    public static bool IsCutsceneActive { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayCutscene(PlayableAsset timelineAsset)
    {
        if (IsCutsceneActive) return;
        IsCutsceneActive = true;

        timelineDirector.playableAsset = timelineAsset;

        cutsceneCamera.Priority = 10;
        //SetBlackBands(true);

        OnAnyCutsceneStart?.Invoke();

        timelineDirector.stopped += OnTimelineStopped;
        
        timelineDirector.Play();
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        IsCutsceneActive = false;
        cutsceneCamera.Priority = 0;
        //SetBlackBands(false);

        timelineDirector.stopped -= OnTimelineStopped;
        
        OnAnyCutsceneEnd?.Invoke();
    }

    private void SetBlackBands(bool active)
    {
        if (active)
        {
            upperBand.transform.DOLocalMoveY(240, 2, false).SetEase(Ease.InOutQuart);
            lowerBand.transform.DOLocalMoveY(-240, 2, false).SetEase(Ease.InOutQuart);
        }
        else
        {
            upperBand.transform.DOLocalMoveY(275, 2, false).SetEase(Ease.InOutQuart);
            lowerBand.transform.DOLocalMoveY(-275, 2, false).SetEase(Ease.InOutQuart);
        }
    }
}
