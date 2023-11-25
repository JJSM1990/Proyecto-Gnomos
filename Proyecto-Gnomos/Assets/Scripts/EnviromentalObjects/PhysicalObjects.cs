using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalObjects : MonoBehaviour, IPushableByPlayer
{
    private Rigidbody m_rigidbody;
    [SerializeField] private bool _requiresMinimumStack;
    [SerializeField] private int _minimumGnomesRequired;

    private void Start()
    {
        m_rigidbody= GetComponent<Rigidbody>();
    }
    public void Push(Vector3 pushStrength, int gnomeOnStack)
    {
        if (_requiresMinimumStack)
        {
            if (gnomeOnStack>= _minimumGnomesRequired)
            {
                m_rigidbody.AddForce(pushStrength*5);
                return;
            }
        }
        m_rigidbody.AddForce(pushStrength);
    }
}
