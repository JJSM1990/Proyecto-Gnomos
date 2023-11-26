using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBoxBehaviour : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private bool _singleActivation;
    private bool CanBeActivated;

    private void Start()
    {
        CanBeActivated = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player"&& CanBeActivated)
        {
            GameManager.Instance.StartTextUI(text);
            if (_singleActivation)
            {
                CanBeActivated = false;
            } 
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

