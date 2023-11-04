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

        if(m_currentState == EnemyState.Chasing)
        {
            ChasePlayer();
        }
        if(m_currentState == EnemyState.Patrolling )
        {
            Patrolling();
        }
        Debug.Log(m_currentState.ToString());

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
        Vector3 rayDir = (m_player.transform.position + m_player.GetComponent<CharacterController>().center) - transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);

        if (Vector3.Angle(transform.forward, rayDir ) < viewAngle && distanceToPlayer < minPlayerDetectionDistance)
        {
            if (Physics.Raycast(transform.position, rayDir, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("Switching to Chasing");
                    m_currentState = EnemyState.Chasing;
                }
            }
        }
        else
        {
            Debug.Log("Switching to Patrol");
            m_currentState = EnemyState.Patrolling;
        }

    }

    private void ChasePlayer()
    {
        m_NavMeshAgent.SetDestination(m_player.transform.position);
    }

    private void OnDrawGizmos()
    {
        //Vector3 rayDir = (m_player.transform.position + m_player.GetComponent<CharacterController>().center) - transform.position;
        //float distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        //if (Vector3.Angle(transform.forward, rayDir) < viewAngle && distanceToPlayer < minPlayerDetectionDistance)
        //{
        //    Gizmos.color = Color.green;
        //} else
        //{
        //    Gizmos.color= Color.red;
        //}

        //Gizmos.DrawRay(new Ray(transform.position, m_player.transform.position - transform.position));
        //Gizmos.DrawSphere(transform.position, minPlayerDetectionDistance);

        
    }


}
