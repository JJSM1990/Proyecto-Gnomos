using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRayCast : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    private GameObject m_lastHit;

    private void Update()
    {
        RaycastToPlayer();
        m_lastHit = m_player;
    }

    private void RaycastToPlayer()
    {
        Vector3 toPlayer = m_player.transform.position - transform.position;
        RaycastHit hit;
        Physics.Raycast( transform.position, toPlayer, out hit, toPlayer.magnitude, -1, QueryTriggerInteraction.Ignore);
        if (hit.collider!=null)
        {
            //if (hit.collider.gameObject!=m_lastHit)
            //{
            //    m_lastHit.GetComponent<TransparencyChanger>()?.ChangeMaterialToNormal();
            //    hit.collider.GetComponent<TransparencyChanger>()?.ChangeMaterialToTransparent();
            //    m_lastHit = hit.collider.gameObject;
            //}
            
        }



}
}
