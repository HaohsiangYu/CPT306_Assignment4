using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public float moveSpeed;
    public Animator m_animator; // 玩家動畫
    public AudioSource m_audioSource; // 玩家音效

    public Vector3 playerInitPosition;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (GameManager.instance.curGameState != GameState.InGame)  // 游戲暫停或結束停止播放音效
        {
            m_audioSource.Pause();
            return;
        }
        if (transform.position.y < 0f)  // 掉出地圖外
        {
            transform.position = playerInitPosition;
        }
        MovePlayer();
    }

    void MovePlayer() {
        float vertical = Input.GetAxis("Vertical"), horizontal = Input.GetAxis("Horizontal");
        Vector3 dir = new Vector3 (vertical, 0f, -horizontal);
        if(dir != Vector3.zero)
        {
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }
            transform.rotation = Quaternion.LookRotation(dir);
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
            m_animator.SetBool("isMove", true); // 跑步動畫
        }
        else
        {
            m_animator.SetBool("isMove", false); // 閑置動畫
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Destination")  // 玩家觸碰終點方塊
        {
            GameManager.instance.CheckWin();
        }
    }

    public Transform GetPlayerTransform()
    {
        return transform;
    }
}
