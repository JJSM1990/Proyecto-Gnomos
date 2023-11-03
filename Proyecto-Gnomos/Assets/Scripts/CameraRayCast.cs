using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraRayCast : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    private GameObject _lastHit;
    private GameManager m_gm;
    private RaycastHit[] m_PreviousHits = new RaycastHit[0];

    private void Start()
    {
        m_gm = GameManager.Instance;
        m_player=m_gm.ReturnPlayer();
        _lastHit = m_player;

    }
    private void Update()
    {
        RaycastToPlayer();

    }

    private void RaycastToPlayer()
    {
        Vector3 toPlayer = m_player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, toPlayer);
        RaycastHit[] newHitArray= Physics.RaycastAll(ray, toPlayer.magnitude, -1, QueryTriggerInteraction.Ignore);
        if (newHitArray.Length>0)
        {
            if (m_PreviousHits.Length>0)
            {
                foreach (RaycastHit oldHit in m_PreviousHits)
                {
                    if (!newHitArray.Contains(oldHit))
                    {
                        oldHit.collider.GetComponent<TransparencyChanger>()?.ChangeMaterialToNormal();
                    }
                }
            }
            foreach (RaycastHit hit in newHitArray)
            {
                if (!m_PreviousHits.Contains(hit))
                {
                    hit.collider.GetComponent<TransparencyChanger>()?.ChangeMaterialToTransparent();
                }
            }
            m_PreviousHits=newHitArray;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, m_player.transform.position - transform.position);
    }
}
