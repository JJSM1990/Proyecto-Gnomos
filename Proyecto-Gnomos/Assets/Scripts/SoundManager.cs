using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioFX
{
    MouseOn,
    GetGnome,
    RunOverGnome,
    DrownedPlayer,
    Tower,
    GnomeStep,
    GameOver,
    Jump,
    MoveObject

}

public enum AudioMusic
{
    IntroMusic,
    GameMusic,
    GameOverMusic
}

public enum AudioAmbience
{
    Car,
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private List<AudioClip> m_fxClips;
    [SerializeField] private List<AudioClip> m_musicClips;
    [SerializeField] private List<AudioClip> m_fxAmbienceClips;

    [SerializeField] public AudioSource musicAudioSource;
    [SerializeField] public AudioSource ambienceAudioSource;
    [SerializeField] public AudioSource clipAudioSource;

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAudioClip(AudioClip audioClip, AudioSource audioSource)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayAudioSource(AudioSource audioSource)
    {
        audioSource.Play();
    }

    public void PlayFx(AudioFX audioFX, AudioSource audioSource)
    {
        audioSource.PlayOneShot(m_fxClips[(int)audioFX]);
    }
    public void PlayMusic(AudioMusic audioMusic, bool isLooping = true)
    {
        musicAudioSource.clip = m_musicClips[(int)audioMusic];
        musicAudioSource.Play();
        SetAudioSourceLoop(musicAudioSource, isLooping);
    }

    public void PlayMusic(AudioMusic audioMusic, AudioSource audioSource, bool isLooping = true)
    {
        audioSource.clip = m_musicClips[(int)audioMusic];
        audioSource.Play();
        SetAudioSourceLoop(audioSource, isLooping);
    }

    public void PlayAmbience(AudioAmbience audioAmbience, bool isLooping = true)
    {
        ambienceAudioSource.clip = m_fxAmbienceClips[(int)audioAmbience];
        ambienceAudioSource.Play();
        SetAudioSourceLoop(ambienceAudioSource, isLooping);
    }

    public void PlayAmbience(AudioAmbience audioAmbience, AudioSource audioSource, bool isLooping = true)
    {
        audioSource.clip = m_fxAmbienceClips[(int)audioAmbience];
        audioSource.Play();
        SetAudioSourceLoop(audioSource, isLooping);
    }

    public void SetAudioSourceLoop(AudioSource audioSource, bool isLoop)
    {
        audioSource.loop = isLoop;
    }

    public void StopAudioSource(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void PauseAudioSource(AudioSource audioSource)
    {
        audioSource.Pause();
    }

    public void MuteAudioSource(AudioSource audioSource)
    {
        audioSource.mute = true;
    }

    public void UnMuteAudioSource(AudioSource audioSource)
    {
        audioSource.mute = false;
    }

    public void ToggleMuteAudioSource(AudioSource audioSource)
    {
        audioSource.mute = !audioSource.mute;
    }

}
