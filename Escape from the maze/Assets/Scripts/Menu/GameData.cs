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
        DontDestroyOnLoad(transform.gameObject);    // �����ГQ���ܱ��Ɖ�
    }

    public void SetGameDifficulty(Difficulty difficulty)
    {
        this.difficulty = difficulty;
    }
}
