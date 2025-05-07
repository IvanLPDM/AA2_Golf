using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    public GameObject menuCanvas;   // El menú completo (Canvas)
    public Button playButton;
    public Button levelSelectButton;
    public Button back;
    public GameObject levels;
   

    void Start()
    {
        // Pausar el juego
        Time.timeScale = 0f;

        // Asignar eventos
        playButton.onClick.AddListener(StartGame);
        levelSelectButton.onClick.AddListener(OpenLevelSelector);
        back.onClick.AddListener(Back);
    }

    void StartGame()
    {
        // Reanudar el juego y ocultar menú
        Time.timeScale = 1f;
        menuCanvas.SetActive(false);
    }

    void Back()
    {
        levels.SetActive(false);
    }

    public void Active()
    {
        menuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    void OpenLevelSelector()
    {
        // Aquí puedes cargar otra escena o mostrar otro panel
        levels.SetActive(true);
    }

    public void InitLevel(string txt)
    {
        SceneManager.LoadScene("Nivel_" + txt);
    }

}
