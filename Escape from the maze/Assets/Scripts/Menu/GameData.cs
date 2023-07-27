using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy, Normal, Hard
}

public class GameData : MonoBehaviour
{
    public Difficulty difficulty;

    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);    // ˆö¾°ÇÐ“Q²»ÄÜ±»ÆÆ‰Ä
    }

    public void SetGameDifficulty(Difficulty difficulty)
    {
        this.difficulty = difficulty;
    }
}
