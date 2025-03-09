using System.Collections;
using System.Collections.Generic;
// using UnityEditor.VersionControl;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip playerHurt;
    [SerializeField] AudioClip playerCritical;
    [SerializeField] AudioClip healthCollected;
    [SerializeField] AudioClip healthNegated;
    [SerializeField] AudioClip playerEliminated;
    private SceneController sceneController;
    private FPSInput fpsInput;
    private readonly int baseHealth = 5;
    private int currentHealth;
    private bool invincible;
    public float shrinkSpeed = 3.0f;

    void Start()
    {
        soundSource.pitch = 1.0f;
        invincible = false;
        currentHealth = baseHealth;
        Messenger<int>.Broadcast(GameEvent.HEALTH_REMAINING, currentHealth);
        sceneController = FindObjectOfType<SceneController>();
        fpsInput = GetComponent<FPSInput>();
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            StartCoroutine(SinkIntoGround());
        }
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.HEALTH_PICKUP_COLLECTED, HealthPickedUp);
        Messenger.AddListener(GameEvent.LEVEL_COMPLETE, ToggleInvincibility);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.HEALTH_PICKUP_COLLECTED, HealthPickedUp);
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETE, ToggleInvincibility);
    }

    public void Hurt(int damage)
    {
        soundSource.PlayOneShot(playerHurt, 1.0f);
        
        if (!invincible)
        {
            currentHealth -= damage;
            Messenger<int>.Broadcast(GameEvent.HEALTH_REMAINING, currentHealth);

            if (currentHealth == 2)
            {
                soundSource.PlayOneShot(playerCritical, 0.6f);
            }

            if (currentHealth == 1)
            {
                soundSource.PlayOneShot(playerCritical, 0.8f);
            }

            if (currentHealth == 0)
            {
                soundSource.PlayOneShot(playerEliminated, 0.9f);
                Messenger.Broadcast(GameEvent.GAME_OVER);
                BGMManager bgmManager = FindObjectOfType<BGMManager>();
                bgmManager.OnPlayerLose();

                PlayerPrefs.SetInt("currentLevel", PlayerPrefs.GetInt("retryLevel"));
                PlayerPrefs.SetFloat("enemySpeed", PlayerPrefs.GetFloat("retryEnemySpeed"));

                fpsInput.enabled = false;
                StartCoroutine(HealthDepleted());
            }
        }
    }

    private void HealthPickedUp()
    {
        if (currentHealth == baseHealth)
        {
            soundSource.PlayOneShot(healthNegated, 0.75f);
        }

        if (currentHealth < baseHealth)
        {
            soundSource.PlayOneShot(healthCollected, 0.75f);

            currentHealth++;
            Messenger<int>.Broadcast(GameEvent.HEALTH_REMAINING, currentHealth);
        }
    }

    private void ToggleInvincibility()
    {
        invincible = true;
    }

    private IEnumerator SinkIntoGround()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);
        if (transform.localScale.y < 0.05f)
        {
            transform.localScale = Vector3.zero;

            yield return null;
        }
    }

    private IEnumerator HealthDepleted()
    {
        yield return new WaitForSeconds(5f);

        sceneController.ResetLevel();
    }
}
