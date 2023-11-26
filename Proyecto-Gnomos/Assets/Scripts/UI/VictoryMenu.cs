using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField] private GameObject VictoryScreen;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        VictoryScreen.SetActive(true);
        SoundManager.Instance.PlayFx(AudioFX.Victory, SoundManager.Instance.clipAudioSource);
    }
}
