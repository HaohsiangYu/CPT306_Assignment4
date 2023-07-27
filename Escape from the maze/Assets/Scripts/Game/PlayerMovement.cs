using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public float moveSpeed;
    public Animator m_animator; // ��҄Ӯ�
    public AudioSource m_audioSource; // �����Ч

    public Vector3 playerInitPosition;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (GameManager.instance.curGameState != GameState.InGame)  // �Α�ͣ��Y��ֹͣ������Ч
        {
            m_audioSource.Pause();
            return;
        }
        if (transform.position.y < 0f)  // �����؈D��
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
            m_animator.SetBool("isMove", true); // �ܲ��Ӯ�
        }
        else
        {
            m_animator.SetBool("isMove", false); // �e�ÄӮ�
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Destination")  // ����|���K�c���K
        {
            GameManager.instance.CheckWin();
        }
    }

    public Transform GetPlayerTransform()
    {
        return transform;
    }
}
