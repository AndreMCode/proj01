using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField] AudioSource bgm00Loop;
    [SerializeField] AudioSource bgm01Loop;
    [SerializeField] AudioSource bgm02Loop;
    [SerializeField] AudioSource bgm03Loop;
    [SerializeField] AudioSource bgm04Loop;
    private AudioSource currentTrack;
    private int currentLevel;

    private bool isFading = false;

    void Start()
    {
        // Set initial defaults / retrieve game data .. needs its own script with priority
        int validLevel = PlayerPrefs.GetInt("validLevel", -1);
        PlayerPrefs.SetInt("validLevel", validLevel);
        int firstPlayLevel = PlayerPrefs.GetInt("retryLevel", 1);
        PlayerPrefs.SetInt("retryLevel", firstPlayLevel);
        float firstPlayEnemySpeed = PlayerPrefs.GetFloat("retryEnemySpeed", 1.0f);
        PlayerPrefs.SetFloat("retryEnemySpeed", firstPlayEnemySpeed);

        if (PlayerPrefs.GetInt("validLevel") > 0)
        { // Pull new currentLevel
            currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        }
        else
        { // Game crashed or player quit, use checkpoint
            currentLevel = PlayerPrefs.GetInt("retryLevel");
            PlayerPrefs.SetInt("currentLevel", PlayerPrefs.GetInt("retryLevel"));
            PlayerPrefs.SetFloat("enemySpeed", PlayerPrefs.GetFloat("retryEnemySpeed"));
        }
        // Invalidate game data until level completed or failed
        PlayerPrefs.SetInt("validLevel", -1);



        // Assign BGM tracks to array
        AudioSource[] bgmTracks = {bgm00Loop, bgm01Loop, bgm02Loop, bgm03Loop, bgm04Loop};
        // Custom BGM track start point
        float startTime = 20.600f;

        // Choose which of five BGM tracks to play dependent on level
        int index = (currentLevel - 1) % 5;
        bgmTracks[index].time = startTime;

        // Play BGM and set reference for fading
        bgmTracks[index].Play();
        currentTrack = bgmTracks[index];
    }

    public void OnPlayerWin()
    { // Long fade on win
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(3.5f));
        }
    }

    public void OnPlayerLose()
    { // Short fade on fail
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(0.25f));
        }
    }

    private IEnumerator FadeOutTracks(float fadeDuration)
    { // Fade function
        isFading = true;

        float startVolume = currentTrack.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);

            currentTrack.volume = newVolume;

            yield return null;
        }

        currentTrack.Stop();

        isFading = false;
    }
}
