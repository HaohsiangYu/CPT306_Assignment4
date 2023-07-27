using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioState
{
    Explore, Escape, Victory, Defeat
}

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    public AudioState curState = AudioState.Explore;
    public AudioSource m_audioSource;
    public AudioClip exploreAudio, escapeAudio, victoryAudio, defeatAudio;
    public GameObject enemies;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        SetAudioState(curState);
        m_audioSource.loop = true;
    }

    private void Update()
    {
        if (GameManager.instance.curGameState == GameState.Pause)
        {
            m_audioSource.Pause();
            return;
        }
        bool beFound = false;
        foreach (Transform enemy in enemies.transform)  // ÅÐ”àÍæ¼ÒÊÇ·ñ±»×·“ô
        {
            if (enemy.GetComponent<Enemy>().curState == EnemyState.Pursue)
            {
                beFound = true;
            }
        }
        if (beFound && curState == AudioState.Explore)
        {
            SetAudioState(AudioState.Escape);
        }
        else
        {
            if (!beFound && curState == AudioState.Escape)
            {
                SetAudioState(AudioState.Explore);
            }
        }
    }

    public void SetAudioState(AudioState state)
    {
        switch (state)
        {
            case AudioState.Explore:
                curState = AudioState.Explore;
                m_audioSource.clip = exploreAudio;
                m_audioSource.Play();
                break;
            case AudioState.Escape:
                curState = AudioState.Escape;
                m_audioSource.clip = escapeAudio;
                m_audioSource.Play();
                break;
            case AudioState.Victory:
                curState = AudioState.Victory;
                m_audioSource.clip = victoryAudio;
                m_audioSource.Play();
                break;
            case AudioState.Defeat:
                curState = AudioState.Defeat;
                m_audioSource.clip = defeatAudio;
                m_audioSource.Play();
                break;
        }
    }
}
