using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject m_player;
    [SerializeField] private TickManager m_tickManager;
    [SerializeField] private GameObject m_activatedList;
    [SerializeField] private GameObject m_deactivatedList;
    [SerializeField] private GameObject m_stackList;
    [SerializeField] private Checkpoints m_lastCheckpoint;
    [SerializeField] private int m_activeGnomesOnLastCheckPoint;
    [SerializeField] private UIManager m_ui;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public GameObject ReturnPlayer()
    {
        return m_player;
    }

    public TickManager ReturnTickManager()
    {
        return m_tickManager;
    }

    public void UpdateCheckpoint(Checkpoints checkpoint, int currentActiveGnomes)
    {
        m_lastCheckpoint= checkpoint;
        m_activeGnomesOnLastCheckPoint= currentActiveGnomes;
    }

    #region RESPAWN
    public void BeginRespawn(CauseOfDeath causeofDeath)
    {
        m_ui.GameOverScreenBegin(causeofDeath, 3f);
        StartCoroutine(RespawnCountdown(3f));
    }

    private void Respawn()
    {
        m_player.transform.position= m_lastCheckpoint.ReturnSpawnPointPosition();
        GameObject gnome;
        for (int i = 0; i < m_activatedList.transform.childCount; i++)
        {
            gnome = m_activatedList.transform.GetChild(i).gameObject;
            gnome.GetComponent<GnomeBrain>().SwitchToFalling();
            gnome.transform.position = m_lastCheckpoint.ReturnSpawnPointPosition()+Random.insideUnitSphere*1f;
        }
        m_ui.GameOverScreenEnd();
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
    #endregion

    public void StartStackUI(int gnomesInRange)
    {
        m_ui.ShowStackGroup(gnomesInRange);
    }

    public void EndStackUI()
    {
        m_ui.HideStackGroup();
    }

    public void UpdateStackUI(float currentCount)
    {
        m_ui.UpdateStackCount(currentCount);
    }

}
