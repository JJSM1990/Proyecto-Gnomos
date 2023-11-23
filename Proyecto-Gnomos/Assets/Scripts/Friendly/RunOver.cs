using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunOver : MonoBehaviour
{
    public Vector3 scaleChange;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Gnome")
        {
            other.gameObject.GetComponent<GnomeBrain>().RunnedOver();   

        }
    }
}
