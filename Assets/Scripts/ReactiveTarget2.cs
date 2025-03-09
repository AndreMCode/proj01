using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReactiveTarget2 : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip receivedDamage1;
    private SceneController sceneController;
    public float reactSpeed = 1000f;
    public float scaleSpeed = 1.5f; // was 3
    private const int baseHealth = 20;
    private int health;
    private bool reacting = false;

    void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
        health = baseHealth;
    }

    void Update()
    {
        if (reacting)
        {
            this.transform.Rotate(reactSpeed / 2 * Time.deltaTime, reactSpeed * Time.deltaTime, 0);

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, scaleSpeed * Time.deltaTime);

            if (transform.localScale.x < 0.05f)
            {
                transform.localScale = Vector3.zero;
                reacting = false;
            }
        }
    }

    public void ReactToHit() // Called by the shooting script
    {
        if (!reacting)
        {
            soundSource.pitch = Random.Range(0.4f, 0.7f); // was 0.5, 0.9
            soundSource.PlayOneShot(receivedDamage1, 1.0f);
            health--;

            if (health <= 0)
            {
                WanderingAI behavior = GetComponent<WanderingAI>();
                if (behavior != null)
                {
                    soundSource.pitch = 0.35f;
                    soundSource.PlayOneShot(receivedDamage1, 2.0f); // was  1.5
                    behavior.SetAlive(false);
                }
                StartCoroutine(Die());
            }
        }
    }

    private IEnumerator Die()
    {
        sceneController.EnemyDown();
        reacting = true;

        if (sceneController != null)
        {
            sceneController.SpawnOne(true);
        }

        yield return new WaitForSeconds(2f); // was 1

        Messenger.Broadcast(GameEvent.ENEMIES_REMAINING);

        Destroy(this.gameObject);
    }
}
