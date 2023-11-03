using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : MonoBehaviour
{

    [SerializeField] NavMeshAgent m_enemy;
    [SerializeField] EnemyState m_enemyCurrentState;
    [SerializeField] float m_enemySpeed;
    private Transform m_target;

    [SerializeField] Transform m_player;

    [SerializeField] Transform[] patrolPoints;
    [SerializeField] int _currentPatrolPoint;

    enum EnemyState
    {
        Patrolling,
        Chasing
    }

    // Start is called before the first frame update
    void Start()
    {
        m_enemyCurrentState = EnemyState.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        m_target = patrolPoints[_currentPatrolPoint];

        if(m_enemyCurrentState == EnemyState.Patrolling)
        {
            if(Vector3.Distance(transform.position, m_target.position) < 0.1f)
            {
                m_target = patrolPoints[_currentPatrolPoint++ % patrolPoints.Length];

                m_enemy.SetDestination(m_target.position);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, m_target.position, m_enemySpeed * Time.deltaTime);
            }
        }
    }
}
