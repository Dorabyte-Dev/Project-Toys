using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    /*public BlockButton playBlock;
    public BlockButton optionsBlock;
    public BlockButton exitBlock;*/


    public void PlayGame()
    {
        Fader.Instance.FadeToScene("Blocking_Prototype");
    }

    public void OpenOptions()
    {
        Debug.Log("OPTIONS pulsado");
        // Aqu� abrir�s tu men� de opciones
    }

    public void ExitGame()
    {
        Debug.Log("EXIT pulsado");
        Application.Quit();
    }
}
