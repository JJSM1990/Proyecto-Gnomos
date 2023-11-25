using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class RunOver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Gnome")
        {
            other.gameObject.GetComponent<GnomeBrain>().RunnedOver();   

        }
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerControl>().Kill(CauseOfDeath.RunOver);
        }
    }
}
