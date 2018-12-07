using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject titleUI;
    [SerializeField]
    private GameObject controlsUI;
    [SerializeField]
    private GameObject creditsUI;

    void Start()
    {
        titleUI.SetActive(true);
        controlsUI.SetActive(false);
        creditsUI.SetActive(false);

    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void DisplayControls()
    {
        titleUI.SetActive(false);
        creditsUI.SetActive(false);
        controlsUI.SetActive(true);
    }

    public void DisplayTitleScreen()
    {
        controlsUI.SetActive(false);
        creditsUI.SetActive(false);
        titleUI.SetActive(true);
    }

    public void DisplayCredits()
    {
        titleUI.SetActive(false);
        controlsUI.SetActive(false);
        creditsUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
