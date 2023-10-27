using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GnomeBrain : MonoBehaviour, UpdateThroughTick
{
    [SerializeField] private GameManager                            m_gameManager;
    [SerializeField] private GameObject                             m_player;
    [SerializeField] private TickManager                            m_tickManager;

    [SerializeField] private NavMeshAgent                           m_navAgent;
    [SerializeField] private float                                  m_minimumDistanceToFollowPlayer;
    private enum GnomeState
    {
        inactive, followingPlayer,Stopped
    }
    private GnomeState _currentGnomeState=GnomeState.inactive;
    private void Start()
    {
        
        m_gameManager=GameManager.Instance;
        m_player = m_gameManager.ReturnPlayer();
        m_tickManager = m_gameManager.ReturnTickManager();
        m_navAgent = GetComponent<NavMeshAgent>();
    }

    

    // Update is called once per frame
    public void UpdateTick()
    {
        float distanceToPlayer = Vector3.Distance(m_player.transform.position, this.gameObject.transform.position);
        switch (_currentGnomeState) 
        {
            case GnomeState.followingPlayer:
                if (distanceToPlayer > m_minimumDistanceToFollowPlayer)
                {
                    NavMeshPath path = new NavMeshPath();
                    m_navAgent.CalculatePath(m_player.transform.position, path);
                    m_navAgent.SetPath(path);
                }
                else
                {
                    _currentGnomeState = GnomeState.Stopped;
                    m_navAgent.isStopped = true;
                }
                break;
            case GnomeState.Stopped:
                if (distanceToPlayer > m_minimumDistanceToFollowPlayer)
                {
                    m_navAgent.isStopped = false;
                    _currentGnomeState = GnomeState.followingPlayer;
                }
                break;
            default:
                break;
        }
    }

    public void Activate()
    {

        if (_currentGnomeState==GnomeState.inactive)
        {
            _currentGnomeState = GnomeState.followingPlayer;
            m_tickManager.AddObjectToAGroup(this.gameObject);
        }
    }
}