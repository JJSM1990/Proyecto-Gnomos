using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyRedirector : MonoBehaviour
{
    private TransparencyChanger m_transChanger;
    

    private void Start()
    {
        m_transChanger=transform.parent.GetComponent<TransparencyChanger>();
    }
    public void MakeTransparent()
    {
        m_transChanger.ChangeMaterialToTransparent();
    }

    public void MakeOpaque()
    {
        m_transChanger.ChangeMaterialToNormal();
    }
}
