using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TransparencyChanger: MonoBehaviour
{
    [SerializeField] private MeshRenderer m_meshRenderer;
    [SerializeField] private Material m_normalMaterial;
    [SerializeField] private Material m_transparentMaterial;
    

    public void ChangeMaterialToTransparent()
    {
        m_meshRenderer.material = m_transparentMaterial;
    }

    public void ChangeMaterialToNormal()
    {
        m_meshRenderer.material = m_normalMaterial;
    }
}
