using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle, Pursue, Back
}
public class Enemy : MonoBehaviour
{
    public EnemyState curState = EnemyState.Idle;
    public Vector2 initPosition, destPosition;
    public float idleSpeed, pursueSpeed;
    public bool goToDest;   // Ѳ߉�r�ГQĿ�ĵ�
    private NavMeshAgent agent;
    private GameObject target = null;

    public MeshFilter FOVMesh;  // ��������ҕҰЧ��
    public float Points;        // �l������Q����
    [Range(1f, 90f)]
    public float FOVAngle;      // ���νǶ�
    public float SightRange;    // ҕҰ����
    List<Vector3> itemList = new List<Vector3>();

    private Animator m_animator;        // ��·���ܲ��Ӯ�
    public GameObject footStep;         // �Ų�λ��
    public AudioSource m_audioSource;   // �Ų���Ч
    public AudioClip walkAudio, runAudio;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 99);
        m_animator = GetComponent<Animator>();
        m_audioSource = footStep.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.curGameState != GameState.InGame)  // �Α�ͣ��Y��ֹͣ������Ч
        {
            m_audioSource.Pause();
            return;
        }
        GenLOSMesh();
        if (!m_audioSource.isPlaying)
        {
            m_audioSource.Play();
        }
        switch (curState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Pursue:
                Pursue();
                break;
            case EnemyState.Back:
                Back();
                break;
        }
    }

    private void SetState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                Debug.Log(gameObject.name + " idle");
                m_audioSource.clip = walkAudio;
                m_animator.SetBool("isPursue", false);
                curState = EnemyState.Idle;
                break;
            case EnemyState.Pursue:
                Debug.Log(gameObject.name + " pursue");
                m_audioSource.clip = runAudio;
                m_animator.SetBool("isPursue", true);
                curState = EnemyState.Pursue;
                break;
            case EnemyState.Back:
                Debug.Log(gameObject.name + " back");
                m_audioSource.clip = walkAudio;
                m_animator.SetBool("isPursue", false);
                curState = EnemyState.Back;
                break;
        }
    }

    private void Idle()
    {
        if (agent.isOnNavMesh)
        {
            agent.speed = idleSpeed;
            if (transform.position.x == initPosition.x && transform.position.z == initPosition.y)   // ǰ��Ŀ�ĵ�
            {
                goToDest = true;
            }
            if (transform.position.x == destPosition.x && transform.position.z == destPosition.y)   // �ص����ɵ�
            {
                goToDest = false;
            }
            agent.destination = goToDest ? new Vector3(destPosition.x, transform.position.y, destPosition.y) 
                : new Vector3(initPosition.x, transform.position.y, initPosition.y);
        }
    }

    private void Pursue()
    {
        if (agent.isOnNavMesh)
        {
            agent.speed = pursueSpeed;
            agent.destination = target.transform.position;  // ׷��Ŀ��
        }
    }

    private void Back()
    {
        if (agent.isOnNavMesh)
        {
            agent.destination = new Vector3(initPosition.x, transform.position.y, initPosition.y);  //�ص����ɵ�
        }
        if (transform.position.x == initPosition.x && transform.position.z == initPosition.y)
        {
            SetState(EnemyState.Idle);
        }
    }

    private List<Vector3> GetSectorial() // ����������Q�Ƕ�
    {
        itemList.Clear();
        for (int j = 0; j <= Points; j++)
        {
            float Angle = -FOVAngle + (((FOVAngle * 2) / Points) * j);
            if (Angle < 0) Angle = 360 + Angle;
            Vector3 Rotation = Quaternion.AngleAxis(Angle, transform.up) * transform.forward;
            itemList.Add(Rotation);
        }
        return itemList;
    }

    private void GenLOSMesh()
    {
        List<Vector3> newVertices = new List<Vector3>();
        newVertices.Add(Vector3.zero);
        GetSectorial();
        
        foreach (Vector3 item in itemList)  // ��������ҕҰ
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, item);
            if (Physics.Raycast(ray, out hit, SightRange))
            {
                if (hit.collider.gameObject.tag == "Player" && curState != EnemyState.Pursue)
                {
                    target = hit.collider.gameObject;
                    SetState(EnemyState.Pursue);
                }
                newVertices.Add(hit.point - transform.position);
            }
            else
            {
                newVertices.Add(ray.GetPoint(SightRange) - transform.position);
            }
        }

        DrawLos(newVertices);
    }
    private void DrawLos(List<Vector3> newVertices) // �������΅^��Ч��
    {
        FOVMesh.mesh.Clear();
        List<Vector2> newUV = new List<Vector2>();
        List<int> newTriangles = new List<int>();
        for (int i = 1; i < newVertices.Count - 1; i++)
        {
            newTriangles.Add(0);
            newTriangles.Add(i);
            newTriangles.Add(i + 1);
        }
        for (int i = 0; i < newVertices.Count; i++)
        {
            newUV.Add(new Vector2(newVertices[i].x, newVertices[i].z));
        }
        FOVMesh.mesh.vertices = newVertices.ToArray();
        FOVMesh.mesh.triangles = newTriangles.ToArray();
        FOVMesh.mesh.uv = newUV.ToArray();
        FOVMesh.transform.rotation = Quaternion.identity;
        FOVMesh.mesh.RecalculateNormals();
    }


    private void OnTriggerExit(Collider other)  // �ɜy����Ƿ��x�_��������
    {
        if (other.gameObject.tag == "Player" && curState == EnemyState.Pursue)
        {
            target = null;
            SetState(EnemyState.Back);
        }
    }

    private void OnCollisionEnter(Collision collision)  // ��������Α�Y��
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.instance.GameOver(false);
        }
    }
}
