using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject m_player;
    [SerializeField] private TickManager m_tickManager;
    [SerializeField] private GameObject m_activatedList;
    [SerializeField] private GameObject m_deactivatedList;
    [SerializeField] private GameObject m_stackList;
    private GameObject m_lastCheckpoint;
    private float m_activeGnomesOnLastCheckPoint;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public GameObject ReturnPlayer()
    {
        return m_player;
    }

    public TickManager ReturnTickManager()
    {
        return m_tickManager;
    }

    public void updateCheckpoint(GameObject checkpoint, float currentActiveGnomes)
    {
        m_lastCheckpoint= checkpoint;
        m_activeGnomesOnLastCheckPoint= currentActiveGnomes;

    }
}
