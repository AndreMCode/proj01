using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using Palmmedia.ReportGenerator.Core;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIController : MonoBehaviour
{
    [SerializeField] TMP_Text remainingLabel;
    [SerializeField] TMP_Text healthLabel;
    [SerializeField] Image healthIcon;
    [SerializeField] TMP_Text currentLevel;
    [SerializeField] Button settingsButton;
    [SerializeField] TMP_Text crosshair;
    [SerializeField] TMP_Text pauseGlyph;
    [SerializeField] TMP_Text pauseIndicator;
    [SerializeField] TMP_Text resumeText;
    [SerializeField] TMP_Text levelCompleteText;
    [SerializeField] TMP_Text getReadyText;
    [SerializeField] TMP_Text gameOverText;
    [SerializeField] SettingsPopup settingsPopup;

    private int enemiesRemaining;
    private int healthRemaining;

    void Start()
    {
        settingsPopup.Close();
        levelCompleteText.enabled = false;
        getReadyText.enabled = false;
        gameOverText.enabled = false;
        currentLevel.text = PlayerPrefs.GetInt("currentLevel", 1).ToString();
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.LEVEL_COMPLETE, LevelComplete);
        Messenger.AddListener(GameEvent.GET_READY, GetReady);
        Messenger.AddListener(GameEvent.GAME_OVER, GameOver);
        Messenger.AddListener(GameEvent.ENEMIES_REMAINING, EnemiesRemaining);
        Messenger<int>.AddListener(GameEvent.SET_ENEMIES_REMAINING, SetEnemiesRemaining);
        Messenger<int>.AddListener(GameEvent.HEALTH_REMAINING, HealthRemaining);
        Messenger.AddListener(GameEvent.CROSSHAIR_ON, CrosshairOn);
        Messenger.AddListener(GameEvent.CROSSHAIR_OFF, CrosshairOff);
        Messenger.AddListener(GameEvent.PAUSE_GLYPH_ON, PauseGlyphOn);
        Messenger.AddListener(GameEvent.PAUSE_GLYPH_OFF, PauseGlyphOff);
        Messenger.AddListener(GameEvent.PAUSE_INDICATOR_ON, PauseIndicatorOn);
        Messenger.AddListener(GameEvent.PAUSE_INDICATOR_OFF, PauseIndicatorOff);
        Messenger.AddListener(GameEvent.SETTINGS_BUTTON_TOGGLE, SettingsButtonToggle);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETE, LevelComplete);
        Messenger.RemoveListener(GameEvent.GET_READY, GetReady);
        Messenger.RemoveListener(GameEvent.GAME_OVER, GameOver);
        Messenger.RemoveListener(GameEvent.ENEMIES_REMAINING, EnemiesRemaining);
        Messenger<int>.RemoveListener(GameEvent.SET_ENEMIES_REMAINING, SetEnemiesRemaining);
        Messenger<int>.RemoveListener(GameEvent.HEALTH_REMAINING, HealthRemaining);
        Messenger.RemoveListener(GameEvent.CROSSHAIR_ON, CrosshairOn);
        Messenger.RemoveListener(GameEvent.CROSSHAIR_OFF, CrosshairOff);
        Messenger.RemoveListener(GameEvent.PAUSE_GLYPH_ON, PauseGlyphOn);
        Messenger.RemoveListener(GameEvent.PAUSE_GLYPH_OFF, PauseGlyphOff);
        Messenger.RemoveListener(GameEvent.PAUSE_INDICATOR_ON, PauseIndicatorOn);
        Messenger.RemoveListener(GameEvent.PAUSE_INDICATOR_OFF, PauseIndicatorOff);
        Messenger.RemoveListener(GameEvent.SETTINGS_BUTTON_TOGGLE, SettingsButtonToggle);
    }

    private void LevelComplete()
    {
        if (levelCompleteText.enabled)
        {
            levelCompleteText.enabled = false;
        }
        else
        {
            levelCompleteText.enabled = true;
        }
    }

    private void GetReady()
    {
        if (getReadyText.enabled)
        {
            getReadyText.enabled = false;
        }
        else
        {
            getReadyText.enabled = true;
        }
    }

    private void GameOver()
    {
        if (gameOverText.enabled)
        {
            gameOverText.enabled = false;
        }
        else
        {
            gameOverText.enabled = true;
        }
    }

    private void SetEnemiesRemaining(int value)
    {
        enemiesRemaining = value;
        remainingLabel.text = value.ToString();
    }

    private void EnemiesRemaining()
    {
        enemiesRemaining -= 1;
        remainingLabel.text = enemiesRemaining.ToString();
    }

    private void HealthRemaining(int value)
    {
        healthRemaining = value;

        if (healthRemaining == 2)
        {
            StartCoroutine(HealthLowIconFlash());
        }

        if (healthRemaining == 1)
        {
            StartCoroutine(HealthCriticalIconFlash());
        }

        if (healthRemaining <= 0)
        {
            healthRemaining = 0;
        }

        healthLabel.text = healthRemaining.ToString();
    }

    private void CrosshairOn()
    {
        crosshair.enabled = true;
    }

    private void CrosshairOff()
    {
        crosshair.enabled = false;
    }

    private void PauseGlyphOn()
    {
        pauseGlyph.enabled = true;
    }

    private void PauseGlyphOff()
    {
        pauseGlyph.enabled = false;
    }

    private void PauseIndicatorOn()
    {
        pauseIndicator.enabled = true;
        resumeText.enabled = true;
    }

    private void PauseIndicatorOff()
    {
        pauseIndicator.enabled = false;
        resumeText.enabled = false;
    }

    private void SettingsButtonToggle()
    {
        if (settingsButton != null && settingsButton.interactable)
        {
            settingsButton.interactable = false;
        }
        else if (settingsButton != null)
        {
            settingsButton.interactable = true;
        }
    }

    public void OnOpenSettings()
    {
        settingsPopup.Open();
    }

    public void OnPointerDown()
    {
        Debug.Log("pointer down");
    }

    private IEnumerator HealthLowIconFlash()
    {
        while (healthRemaining == 2)
        {
            healthIcon.color = new Color(1f, 1f, 1f, 1f);

            yield return new WaitForSeconds(0.4f);
            healthIcon.color = new Color(0.7529412f, 0f, 0f, 1f);

            yield return new WaitForSeconds(0.4f);
            yield return null;
        }
        healthIcon.color = new Color(0.7529412f, 0f, 0f, 1f);
        yield return null;
    }

    private IEnumerator HealthCriticalIconFlash()
    {
        while (healthRemaining == 1)
        {
            healthIcon.color = new Color(1f, 1f, 1f, 1f);

            yield return new WaitForSeconds(0.1f);
            healthIcon.color = new Color(0.7529412f, 0f, 0f, 1f);

            yield return new WaitForSeconds(0.1f);
            yield return null;
        }
        healthIcon.color = new Color(0.7529412f, 0f, 0f, 1f);
        yield return null;
    }
}
