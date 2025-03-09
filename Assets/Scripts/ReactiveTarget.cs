using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReactiveTarget : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip receivedDamage1;
    // [SerializeField] AudioClip receivedDamage2;
    // [SerializeField] AudioClip receivedDamage3;
    // [SerializeField] AudioClip receivedDamage4;
    private SceneController sceneController;
    public float reactSpeed = 1000f;
    public float scaleSpeed = 3f;
    private bool reacting = false;

    void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
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
            WanderingAI behavior = GetComponent<WanderingAI>();
            if (behavior != null)
            {
                // int soundDecider = Random.Range(0, 4);
                // if (soundDecider == 0)
                // {
                //     soundSource.PlayOneShot(receivedDamage1);
                // }
                // else if (soundDecider == 1)
                // {
                //     soundSource.PlayOneShot(receivedDamage2);
                // }
                // else if (soundDecider == 2)
                // {
                //     soundSource.PlayOneShot(receivedDamage3);
                // }
                // else if (soundDecider == 3)
                // {
                //     soundSource.PlayOneShot(receivedDamage4);
                // }

                soundSource.pitch = Random.Range(0.5f, 0.9f);
                soundSource.PlayOneShot(receivedDamage1, 1.5f);
                behavior.SetAlive(false);
            }
            StartCoroutine(Die());
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

        yield return new WaitForSeconds(1f);

        Messenger.Broadcast(GameEvent.ENEMIES_REMAINING);

        Destroy(this.gameObject);
    }
}
