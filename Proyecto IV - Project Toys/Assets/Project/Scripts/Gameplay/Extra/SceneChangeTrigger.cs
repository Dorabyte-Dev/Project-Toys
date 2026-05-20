using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour
{
    public string sceneName;
    public void TriggerChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
