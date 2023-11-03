using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalObjects : MonoBehaviour, IPushableByPlayer
{
    private Rigidbody m_rigidbody;

    private void Start()
    {
        m_rigidbody= GetComponent<Rigidbody>();
    }
    public void Push(Vector3 pushStrength)
    {
        m_rigidbody.AddForce(pushStrength);
    }
}
