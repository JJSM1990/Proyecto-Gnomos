using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;

    private void Start()
    {
        SoundManager.Instance.PlayMusic(AudioMusic.IntroMusic, true);
    }
    public void OnPlayClicked()
    {
        SceneManager.LoadScene(Scenes.LOADING_SCENE);
        SoundManager.Instance.StopAudioSource(SoundManager.Instance.musicAudioSource);
    }

    public void OnExitClicked()
    {
        Debug.Log("ExitGame");
        Application.Quit();
    }

    public void OnButton()
    {
        SoundManager.Instance.PlayFx(AudioFX.MouseOn, SoundManager.Instance.clipAudioSource);
    }



}
