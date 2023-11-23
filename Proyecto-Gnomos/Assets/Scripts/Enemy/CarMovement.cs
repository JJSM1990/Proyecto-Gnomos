using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CarMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent m_NavMeshAgent;
    [SerializeField] Transform[] m_patrolPoints;
    [SerializeField] int _currentPatrolPoint;

    [SerializeField] float viewAngle;
    [SerializeField] float minPlayerDetectionDistance;
    [SerializeField] bool m_playerIsSeen;

    [SerializeField] GameObject m_player;
    [SerializeField] EnemyState m_currentState;
    //[SerializeField] bool m_isChasing;

    private Coroutine m_preparingCoroutine;
    [SerializeField] float m_preparingTime;
    [SerializeField] GameObject m_particles;



    private enum EnemyState
    {
        Patrolling,
        Chasing,
        Preparing,
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentPatrolPoint = 0;
        m_currentState = EnemyState.Patrolling;
        m_NavMeshAgent.speed = 3.5f;
        m_NavMeshAgent.angularSpeed = 160f;
        m_particles.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerOnSight();
        ChangeState();
        if(m_currentState == EnemyState.Chasing)
        {
            ChasePlayer();
        }
        if(m_currentState == EnemyState.Patrolling )
        {
            Patrolling();
        }
        if(m_currentState == EnemyState.Preparing)
        {
            PreparingToChase();
        }

    }

    private void Patrolling()
    {
        m_NavMeshAgent.speed = 3.5f;
        m_NavMeshAgent.angularSpeed = 160;
        if (!m_NavMeshAgent.hasPath)
        {
            m_NavMeshAgent.SetDestination(m_patrolPoints[_currentPatrolPoint].position);
            _currentPatrolPoint++;

            if (_currentPatrolPoint > m_patrolPoints.Length - 1)
            {
                _currentPatrolPoint = 0;
            }
        }
    }

    private void PlayerOnSight()
    {
        RaycastHit hit;
        Vector3 rayDir = (m_player.transform.position + m_player.GetComponent<CharacterController>().center) -transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        if (Vector3.Angle(transform.forward, rayDir ) < viewAngle && distanceToPlayer < minPlayerDetectionDistance)
        {

            if (Physics.Raycast(transform.position, rayDir, out hit,10000, -1, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("Raycast");
                    m_playerIsSeen = true;
                    return;
                }
            }
        }
        m_playerIsSeen = false;
    }

    private void ChasePlayer()
    {
        m_NavMeshAgent.speed = 20f;
        m_NavMeshAgent.angularSpeed = 360f;
        m_NavMeshAgent.acceleration = 20f;
        m_NavMeshAgent.SetDestination(m_player.transform.position);
    }

    private void PreparingToChase() 
    {
        if(m_preparingCoroutine == null)
        {
            m_preparingCoroutine = StartCoroutine(PreparingCorutine());
        }
  
    }
    private void ChangeState()
    {
        switch (m_currentState)
        {
            case EnemyState.Patrolling:
                if(m_playerIsSeen == true)
                {
                    m_currentState = EnemyState.Preparing;
                }
                break;
            case EnemyState.Chasing:
                if(m_playerIsSeen == false)
                {
                    m_particles.SetActive(false);
                    m_currentState=EnemyState.Patrolling;
                }
                break;
            default:
                break;

        }
    }

    private IEnumerator PreparingCorutine()
    {
        m_currentState = EnemyState.Preparing;
        m_NavMeshAgent.speed = 0;
        m_particles.SetActive(true);
        yield return new WaitForSeconds(m_preparingTime);
        Debug.Log("Switching to chase");
        m_currentState = EnemyState.Chasing;
        m_preparingCoroutine = null;   
    }
}
