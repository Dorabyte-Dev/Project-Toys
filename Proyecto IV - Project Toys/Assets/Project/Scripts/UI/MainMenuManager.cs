using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public BlockButton playBlock;
    public BlockButton optionsBlock;
    public BlockButton exitBlock;

    void Start()
    {
        playBlock.onClick = PlayGame;
        optionsBlock.onClick = OpenOptions;
        exitBlock.onClick = ExitGame;
    }

    void PlayGame()
    {
        Debug.Log("PLAY pulsado");
        // Aquí cargarás la escena del juego
        // SceneManager.LoadScene("GameScene");
    }

    void OpenOptions()
    {
        Debug.Log("OPTIONS pulsado");
        // Aquí abrirás tu menú de opciones
    }

    void ExitGame()
    {
        Debug.Log("EXIT pulsado");
        Application.Quit();
    }
}
