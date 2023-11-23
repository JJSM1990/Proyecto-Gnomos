using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;


    public void OnPlayClicked()
    {
        SceneManager.LoadScene(Scenes.LOADING_SCENE);
    }

    public void OnExitClicked()
    {
        Debug.Log("ExitGame");
        Application.Quit();
    }


}
