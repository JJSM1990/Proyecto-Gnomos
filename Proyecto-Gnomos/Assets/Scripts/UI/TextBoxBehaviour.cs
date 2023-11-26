using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBoxBehaviour : MonoBehaviour
{
    [SerializeField] private string text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player")
        {
            GameManager.Instance.StartTextUI(text);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.EndTextUI();
        }
    }
}

