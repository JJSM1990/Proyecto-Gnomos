using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // VARIABLES PARA EL MENU DE PAUSA
    [SerializeField] private GameObject             m_pauseMenu;


    // VARIABLES PARA EL STACKMETRO
    [Header("Stack variables")]
    [SerializeField] private GameObject             m_stackMeterGroup;
    [SerializeField] private GameObject             m_stackHand;
    private int                                     _maxPossibleGnomes;
    private float                                   _currentGnomes;
    private float                                   _stackMeterFill;
    private Vector3                                 _stackHandRotation;

    //VARIABLES PARA EL NUMERO DE GNOMOS
    [SerializeField] private string                 m_gnomesTotal;
    [SerializeField] private GameObject             m_numStackGnomes;
    [SerializeField] private GameObject             m_numActiveGnomes;
    [SerializeField] private TMP_Text               m_currentGnomesText;


    // VARIABLES PARA EL GAME OVER
    public enum CauseOfDeath { Drowning, RunOver }

    [Header("Game Over Variables")]
    [SerializeField] private Image                  m_gameOverImage;
    [SerializeField] private GameObject             m_livesLeftGroup;
    [SerializeField] private TextMeshProUGUI        m_livesLeftText;

    [Header("Game Over Images")]
    [SerializeField] private Sprite                 m_drowningDeathSplash;
    [SerializeField] private Sprite                 m_carDeathSplash;

    //VARIABLES PARA EL MENU NOLIVES

    [Header("No Lives Variables")]
    [SerializeField] private GameObject             m_noLivesGroup;
    [SerializeField] private GameObject             m_drowningExitButton;
    [SerializeField] private GameObject             m_carExitButton;

    private void Update()
    {
        NumberOfGnomes();
    }

    #region PAUSEMENU
    public void Pause()
    {
        m_pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        m_pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Scenes.MAIN_MENU_SCENE);
    }
    #endregion

    public void NumberOfGnomes()
    {
        m_gnomesTotal = (m_numStackGnomes.transform.childCount + m_numActiveGnomes.transform.childCount).ToString();

        m_currentGnomesText.text = m_gnomesTotal;
    }


    #region STACK
    public void ShowStackGroup(int gnomesInRange)
    {
        StartStackCount(gnomesInRange);
        if (_maxPossibleGnomes > 0) m_stackMeterGroup.SetActive(true);
    }

    public void HideStackGroup()
    {
        m_stackMeterGroup.SetActive(false);
    }

    public void StartStackCount(int gnomesInRange)
    {
        _maxPossibleGnomes= gnomesInRange;
    }

    public void UpdateStackCount(float currentCount)
    {
        _currentGnomes= currentCount;
        UpdateStackMeterFill();
        UpdateStackHand();
    }

    private void UpdateStackMeterFill()
    {
        _stackMeterFill = Mathf.Clamp(_currentGnomes / _maxPossibleGnomes, 0, 1);
    }

    private void UpdateStackHand()
    {
        _stackHandRotation.z = _stackMeterFill * -180;
        m_stackHand.transform.rotation = Quaternion.Euler(_stackHandRotation);
    }
    #endregion

    #region GAMEOVER
    public void GameOverScreenBegin(CauseOfDeath causeOfDeath, float timer, int livesLeft)
    {
        m_gameOverImage.gameObject.SetActive(true);
        SoundManager.Instance.PauseAudioSource(SoundManager.Instance.musicAudioSource);
        switch (causeOfDeath)
        {
            case CauseOfDeath.Drowning:
                m_gameOverImage.sprite = m_drowningDeathSplash;
                SoundManager.Instance.PlayFx(AudioFX.DrownedPlayer, SoundManager.Instance.clipAudioSource);
                break;
            case CauseOfDeath.RunOver:
                m_gameOverImage.sprite = m_carDeathSplash;
                SoundManager.Instance.PlayFx(AudioFX.RunOverGnome, SoundManager.Instance.clipAudioSource);
                break;
        }
        StartCoroutine(TurnOnScreen(timer, livesLeft));
    }

    public void GameOverScreenEnd()
    {
        Color color = m_gameOverImage.color;
        color.a = 0f;
        m_gameOverImage.color = color;
        m_gameOverImage.gameObject.SetActive(false);
        m_livesLeftGroup.SetActive(false);
    }

    public void UpdateLives(int livesLeft)
    {
        m_livesLeftGroup.SetActive(true);
        m_livesLeftText.text = livesLeft-1 + " LIVES LEFT";
    }
    private IEnumerator TurnOnScreen(float timer, int livesLeft)
    {
        float time = 0f;
        float alpha = 0f;
        Color color = m_gameOverImage.color;
        color.a = alpha;
        m_gameOverImage.color = color;
        while (alpha < 1)
        {
            alpha = time * 2 / (timer);
            color.a = alpha;
            m_gameOverImage.color = color;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (livesLeft > 0) UpdateLives(livesLeft);
        color.a = 1f;
        SoundManager.Instance.PlayFx(AudioFX.GameOver, SoundManager.Instance.clipAudioSource);
        m_gameOverImage.color = color;
    }

    public void NoLivesLeft(CauseOfDeath causeOfDeath)
    {
        m_noLivesGroup.SetActive(true);
        switch (causeOfDeath)
        {
            case CauseOfDeath.Drowning:
                m_drowningExitButton.SetActive(true);
                break;
            case CauseOfDeath.RunOver:
                m_carExitButton.SetActive(true);
                break;
        }
    }
    #endregion
}
