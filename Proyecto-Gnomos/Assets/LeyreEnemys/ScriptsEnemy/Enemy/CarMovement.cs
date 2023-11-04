using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    [SerializeField] GameObject m_player;
    [SerializeField] EnemyState m_currentState;
    //[SerializeField] bool m_isChasing;



    private enum EnemyState
    {
        Patrolling,
        Chasing,
        Attack,
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentPatrolPoint = 0;
        m_currentState = EnemyState.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerOnSight();

        if(m_currentState == EnemyState.Chasing && m_currentState != EnemyState.Patrolling)
        {
            ChasePlayer();
        }
        if(m_currentState == EnemyState.Patrolling && m_currentState != EnemyState.Chasing)
        {
            Patrolling();
        }

    }

    private void Patrolling()
    {
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
        Vector3 rayDir = m_player.transform.position - transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);

        if (Vector3.Angle(rayDir, transform.forward) < viewAngle)
        {
            if (Physics.Raycast(transform.position, rayDir, out hit) && distanceToPlayer < minPlayerDetectionDistance)
            {
                if (hit.transform.tag == "Player")
                {
                    m_currentState = EnemyState.Chasing;
                }
            }
        }
        else if(distanceToPlayer > minPlayerDetectionDistance)
        {
            m_currentState = EnemyState.Patrolling;
        }

    }

    private void ChasePlayer()
    {
        m_NavMeshAgent.SetDestination(m_player.transform.position);
    }




}
