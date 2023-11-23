using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    private AsyncOperation m_LoadingOperation;

    private void OnEnable()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForEndOfFrame();
        m_LoadingOperation = SceneManager.LoadSceneAsync(Scenes.LEVEL_SCENE, LoadSceneMode.Single);
        m_LoadingOperation.allowSceneActivation = false;

        while (!(m_LoadingOperation.progress >= 0.9f)) 
        {
            Debug.Log(m_LoadingOperation.progress.ToString());
            yield return null;
        }

        FinishLoading();
    }

    private void FinishLoading()
    {
        m_LoadingOperation.allowSceneActivation = true;
    }

}
