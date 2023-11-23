using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunOver : MonoBehaviour
{
    public Vector3 scaleChange;

    private void Awake()
    {
        scaleChange = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Car")
        {
            scaleChange.y = 0.1f;
        }
    }
}
