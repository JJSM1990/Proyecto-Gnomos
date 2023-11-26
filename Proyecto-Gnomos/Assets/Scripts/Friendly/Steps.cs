using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steps : MonoBehaviour
{
    public void Step()
    {
        SoundManager.Instance.PlayFx(AudioFX.GnomeStep, SoundManager.Instance.clipAudioSource);
    }
}
