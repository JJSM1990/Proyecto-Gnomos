using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRayCast : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    

    private void Update()
    {
        RaycastToPlayer();
    }

    private void RaycastToPlayer()
    {
        Vector3 toPlayer = m_player.transform.position - transform.position;
        Ray ray = new Ray(toPlayer, transform.position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        if (hit.collider!=null)
        {
            Debug.Log(hit.collider.name);
        }
        
    }
}
