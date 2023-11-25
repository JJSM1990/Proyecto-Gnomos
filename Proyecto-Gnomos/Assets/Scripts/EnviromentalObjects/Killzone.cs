using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class Killzone : MonoBehaviour
{
    private Collider m_collider;
    [SerializeField] private CauseOfDeath _causeOfDeath;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerControl>().Kill(_causeOfDeath);
        }
    }
}
