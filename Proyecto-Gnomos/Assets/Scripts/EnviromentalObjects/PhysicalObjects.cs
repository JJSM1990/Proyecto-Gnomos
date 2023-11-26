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
        m_rigidbody.isKinematic = _requiresMinimumStack;
    }
    public void Push(Vector3 pushStrength, int gnomeOnStack)
    {
        if (_requiresMinimumStack)
        {
            if (gnomeOnStack>= _minimumGnomesRequired)
            {
                m_rigidbody.isKinematic=false;
                m_rigidbody.AddForce(pushStrength*5);
                return;
            }else
            {
                m_rigidbody.isKinematic = true;
            }
        }
        m_rigidbody.AddForce(pushStrength);
    }
}
