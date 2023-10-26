using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject m_player;
    [SerializeField] private TickManager m_tickManager;

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
}
