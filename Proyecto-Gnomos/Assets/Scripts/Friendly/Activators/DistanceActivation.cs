using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceActivation : MonoBehaviour
{
    [SerializeField] private GnomeBrain _gnomeBrain;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerControl>() != null)
        {
            _gnomeBrain.Activate();
        }
    }
}
