using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class Fader : MonoBehaviour
{
    public static Fader Instance;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();

       
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, 1f);
    }

    public void FadeToScene(string sceneName)
    {
        canvasGroup.DOFade(1f, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }
}
