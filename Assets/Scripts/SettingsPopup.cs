using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Button restartGameButton;
    [SerializeField] TMP_Text highestLevelLabel;

    void Start()
    {
        highestLevelLabel.text = PlayerPrefs.GetInt("highestLevel", 1).ToString();
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
    }
    public void Open()
    {
        gameObject.SetActive(true);
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
        PlayerPrefs.SetInt("retryLevel", 1);
        PlayerPrefs.SetFloat("retryEnemySpeed", 1.0f);
        PlayerPrefs.SetInt("currentLevel", 1);
        PlayerPrefs.SetFloat("enemySpeed", 1.0f);
        Messenger.Broadcast(GameEvent.RESET_GAME);
    }

    public void OnSensitivityValue(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        Debug.Log($"Mouse Sensitivity: {sensitivity}");
    }
}
