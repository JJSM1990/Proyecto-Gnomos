using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TransparencyChanger: MonoBehaviour
{
    private MeshRenderer m_meshRenderer;
    [SerializeField] private Material m_normalMaterial;
    [SerializeField] private Material m_transparentMaterial;

    private void Start()
    {
        m_meshRenderer= GetComponent<MeshRenderer>();
        m_meshRenderer.material= m_normalMaterial;
    }
    public void ChangeMaterialToTransparent()
    {
        m_meshRenderer.material = m_transparentMaterial;
    }

    public void ChangeMaterialToNormal()
    {
        m_meshRenderer.material = m_normalMaterial;
    }
}
