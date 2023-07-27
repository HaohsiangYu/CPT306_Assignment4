using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    InGame, Pause, GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int key, numOfKey; // 已搜集鑰匙數和鑰匙總數
    public TMP_Text Text_Difficulty, Text_Time, Text_Key, Text_GameoverDetail;
    public Canvas ingameCanvas, pauseCanvas, gameoverCanvas;
    public GameState curGameState = GameState.InGame;
    private float time = 0.0f;
    private int minutes, seconds;
    private bool gameover = false;
    public AudioSource m_audioSource;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && curGameState != GameState.GameOver)   // 按Esc鍵暫停和繼續游戲
        {
            GamePause();
        }
    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        minutes = (int)time / 60;
        seconds = (int)time % 60;
        Text_Time.text = string.Format("Time: {0:D2}:{1:D2}", minutes, seconds);
    }

    private void SetGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.InGame:
                ingameCanvas.enabled = true;
                pauseCanvas.enabled = false;
                gameoverCanvas.enabled = false;
                break;
            case GameState.Pause:
                ingameCanvas.enabled = true;
                pauseCanvas.enabled = true;
                gameoverCanvas.enabled = false;
                break;
            case GameState.GameOver:
                ingameCanvas.enabled = true;
                pauseCanvas.enabled = false;
                gameoverCanvas.enabled = true;
                break;
        }
        curGameState = gameState;
    }

    public void StartGame()
    {
        SetGameState(GameState.InGame);
        switch (GameObject.Find("GameData").GetComponent<GameData>().difficulty)
        {
            case Difficulty.Easy:
                numOfKey = 1;
                Text_Difficulty.text += "Easy";
                break;
            case Difficulty.Normal:
                Text_Difficulty.text += "Normal";
                numOfKey = 2; 
                break;
            case Difficulty.Hard:
                Text_Difficulty.text += "Hard";
                numOfKey = 3;
                break;
        }
        AudioController.instance.SetAudioState(AudioState.Explore);
        Time.timeScale = 1;
        gameover = false;
        Text_Key.text = string.Format("Key: {0:G} / {1:G}", key, numOfKey);
    }

    public void GamePause()
    {
        m_audioSource.Play();
        SetGameState(curGameState == GameState.InGame ? GameState.Pause : GameState.InGame);
        Time.timeScale = 1 - Time.timeScale;
    }

    public void Restart()
    {
        m_audioSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        m_audioSource.Play();
        Destroy(GameObject.Find("GameData"));   // 銷毀本次游戲的gameData避免重複
        SceneManager.LoadScene("MenuScene");
    }

    public void GameOver(bool win)
    {
        if (!gameover)
        {
            gameover = true;
            if (win)
            {
                AudioController.instance.SetAudioState(AudioState.Victory);
                Text_GameoverDetail.text = string.Format("You escape the maze!\r\n\r\nCost Time: {0:D2}:{1:D2}", minutes, seconds);
            }
            else
            {
                AudioController.instance.SetAudioState(AudioState.Defeat);
                Text_GameoverDetail.text = string.Format("You have been caught!\r\nCollect Key: {0:G} / {1:G}\r\nSurvival Time: {2:D2}:{3:D2}", key, numOfKey, minutes, seconds);
            }
            SetGameState(GameState.GameOver);
            Time.timeScale = 0;
        }
    }

    public void AddKey()
    {
        key++;
        Text_Key.text = string.Format("Key: {0:G} / {1:G}", key, numOfKey);
    }

    public void CheckWin()
    {
        if (key == numOfKey)
        {
            GameOver(true);
        }
    }
}
