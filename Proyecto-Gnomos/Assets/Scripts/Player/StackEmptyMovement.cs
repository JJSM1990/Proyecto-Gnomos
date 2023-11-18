using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackEmptyMovement : MonoBehaviour
{
    [SerializeField] private GameObject m_player;

    private void Update()
    {
        transform.position = m_player.transform.position;
    }
}
