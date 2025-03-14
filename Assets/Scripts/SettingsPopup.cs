using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TMP_Text highestLevelLabel;
    [SerializeField] GameObject levelSelectorButtonObject;
    [SerializeField] GameObject levelSelectorInputObject;

    void Start()
    { // Get/set highest level and mouse sensitivity
        highestLevelLabel.text = PlayerPrefs.GetInt("highestLevel", 1).ToString();
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
    }
    public void Open()
    {
        gameObject.SetActive(true);
        levelSelectorInputObject.SetActive(false);
        levelSelectorButtonObject.SetActive(true);

        Messenger.Broadcast(GameEvent.POPUP_IS_OPEN);
        Messenger.Broadcast(GameEvent.PAUSE_INDICATOR_OFF);
    }

    public void Close()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", PlayerPrefs.GetFloat("MouseSensitivity", 1.0f));
        Messenger.Broadcast(GameEvent.EXIT_POPUP_MENU);
        gameObject.SetActive(false);
    }

    public void OnButtonPress()
    {
        PlayerPrefs.SetInt("usingHack", -1);

        PlayerPrefs.SetInt("retryLevel", 1);
        PlayerPrefs.SetFloat("retryEnemySpeed", 1.0f);
        PlayerPrefs.SetInt("currentLevel", 1);
        PlayerPrefs.SetFloat("enemySpeed", 1.0f);
        Messenger.Broadcast(GameEvent.RESET_GAME);
    }

    public void OnResetHighestButtonPress()
    {
        PlayerPrefs.SetInt("highestLevel", 0);
        highestLevelLabel.text = PlayerPrefs.GetInt("highestLevel", 1).ToString();
        Debug.Log("HighestLevel reset");
    }

    public void OnQuitButtonPress()
    {
        Debug.Log("Player quit.");
        Application.Quit();
    }

    public void OnSecretButtonPress()
    {
        levelSelectorButtonObject.SetActive(false);
        levelSelectorInputObject.SetActive(true);
    }

    public void OnSubmitLevel(string input)
    {
        if (int.TryParse(input, out int number) && number >= 1 && number <= 50)
        {
            Debug.Log("Used level selector (" + number + ")");
            PlayerPrefs.SetInt("usingHack", 1);

            // Validate level change
            PlayerPrefs.SetInt("validLevel", 1);

            PlayerPrefs.SetInt("retryLevel", 1);
            PlayerPrefs.SetFloat("retryEnemySpeed", 1.0f);
            PlayerPrefs.SetInt("currentLevel", number);
            PlayerPrefs.SetFloat("enemySpeed", number * 1.0f * 0.01f + 1);

            Messenger.Broadcast(GameEvent.RESET_GAME);
        }
        else
        {
            levelSelectorInputObject.SetActive(false);
            levelSelectorButtonObject.SetActive(true);
        }
    }

    public void OnSensitivityValue(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        Debug.Log($"Mouse Sensitivity: {sensitivity}");
    }
}
