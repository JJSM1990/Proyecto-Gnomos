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
    [SerializeField] private GameObject m_lastCheckpoint;
    [SerializeField] private int m_activeGnomesOnLastCheckPoint;

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

    public void updateCheckpoint(GameObject checkpoint, int currentActiveGnomes)
    {
        m_lastCheckpoint= checkpoint;
        m_activeGnomesOnLastCheckPoint= currentActiveGnomes;
    }

    public void BeginRespawn()
    {
        StartCoroutine(RespawnCountdown(1f));
    }

    private void Respawn()
    {
        int gnomeTarget = m_activeGnomesOnLastCheckPoint; 
        m_player.transform.position=m_lastCheckpoint.transform.position;
        GameObject gnome;
        while (gnomeTarget>0)
        {
            gnome = m_deactivatedList.transform.GetChild(0).gameObject;
            gnome.transform.position = m_lastCheckpoint.transform.position;
            gnome.GetComponent<GnomeBrain>().Activate();
            gnomeTarget--;
        }
        m_player.GetComponent<PlayerControl>().Respawn();
    }

    private IEnumerator RespawnCountdown(float respawnTimer)
    {
        float time = 0f;
        while (time < respawnTimer)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Respawn();
    }
}
