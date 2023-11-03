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
        if(m_currentState == EnemyState.Chasing && m_currentState != EnemyState.Patrolling)
        {
            ChasePlayer();
        }
        else
        {
            Patrolling();
        }

        //NO FUNCIONA 
        //RaycastHit hit;
        //Vector3 rayDirection = m_player.transform.position - transform.position;
        //var distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);

        //if((Vector3.Angle(rayDirection, transform.forward)) < viewAngle)
        //{
        //    if (Physics.Raycast(transform.position, rayDirection, out hit))
        //    {
        //        if (hit.transform.tag == "Player")
        //        {
        //            ChasePlayer();
        //        }
        //    }
        //}

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

    private void ChasePlayer()
    {
        m_NavMeshAgent.SetDestination(m_player.transform.position);
    }


}
