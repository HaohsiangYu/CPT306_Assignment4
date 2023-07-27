using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuState
{
    Menu, Help, Acknowledge, Difficulty
}

public class MenuManager : MonoBehaviour
{
    public Canvas menuCanvas, helpCanvas, acknowledgeCanvas, difficultyCanvas;
    public MenuState curMenuState = MenuState.Menu;
    public AudioSource m_audiosource;   // 按鍵音效
    public GameObject gameData;

    private void Start()
    {
        SetMenuState(MenuState.Menu);
    }

    public void StartGame()
    {
        SetMenuState(MenuState.Difficulty);
    }

    private void Update()
    {
        if (curMenuState != MenuState.Menu && Input.GetKey(KeyCode.Escape)) // 按Esc鍵返回
        {
            BackToMenu();
        }
    }

    public void StartGameDifficulty(int level)  // 根據選擇難度更改gameData參數
    {
        gameData.GetComponent<GameData>().SetGameDifficulty((Difficulty) level);
        m_audiosource.Play();
        SceneManager.LoadScene("MazeScene");
    }

    public void SetMenuState(MenuState state)
    {
        m_audiosource.Play();
        switch (state)
        {
            case MenuState.Menu:
                curMenuState = MenuState.Menu;
                menuCanvas.enabled = true;
                helpCanvas.enabled = false;
                acknowledgeCanvas.enabled = false;
                difficultyCanvas.enabled = false;
                break;
            case MenuState.Help:
                curMenuState = MenuState.Help;
                menuCanvas.enabled = true;
                helpCanvas.enabled = true;
                acknowledgeCanvas.enabled = false;
                difficultyCanvas.enabled = false;
                break;
            case MenuState.Acknowledge:
                curMenuState = MenuState.Acknowledge;
                menuCanvas.enabled = true;
                helpCanvas.enabled = false;
                acknowledgeCanvas.enabled = true;
                difficultyCanvas.enabled = false;
                break;
            case MenuState.Difficulty:
                curMenuState = MenuState.Difficulty;
                menuCanvas.enabled = true;
                helpCanvas.enabled = false;
                acknowledgeCanvas.enabled = false;
                difficultyCanvas.enabled = true;
                break;
        }
    }

    public void ApplicationQuit()
    {
        m_audiosource.Play();
        Application.Quit();
    }

    public void GetHelpCanvas()
    {
        SetMenuState(MenuState.Help);
    }

    public void GetAcknowledgeCanvas()
    {
        SetMenuState(MenuState.Acknowledge);
    }

    public void BackToMenu()
    {
        SetMenuState(MenuState.Menu);
    }
}
