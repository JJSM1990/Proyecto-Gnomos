using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent m_NavMeshAgent;
    [SerializeField] Transform[] m_patrolPoints;
    [SerializeField] int _currentPatrolPoint;

    [SerializeField] float degrees;

    [SerializeField] GameObject m_player;
    [SerializeField] EnemyState m_currentState;

    private enum EnemyState
    {
        Patrolling,
        Chasing,
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
        if(m_currentState == EnemyState.Patrolling)
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

        if(m_currentState == EnemyState.Chasing)
        {
            Vector3 direction = m_player.transform.position - transform.position;
            if(Mathf.Abs(Vector3.Angle(transform.forward, direction)) < degrees)
            {
                m_NavMeshAgent.SetDestination(m_player.transform.position);
            }

        }

    }


}
