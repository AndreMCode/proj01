using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip healthSpawn;
    public float speed = 1.0f;
    public float maxY = 1.5f;
    public float minY = 0.5f;

    private const int direction = 1;
    private bool bounced;

    void Start()
    { // Play sound and initiate translation check
        soundSource.PlayOneShot(healthSpawn);
        bounced = false;
    }

    void Update()
    { // Translate up and down
        transform.Rotate(0, speed * 100.0f * Time.deltaTime, 0, Space.Self);

        if (!bounced)
        {
            transform.Translate(0, direction * speed * Time.deltaTime, 0);
            if (transform.localPosition.y > maxY)
            {
                bounced = true;
            }
        }

        if (bounced)
        {
            transform.Translate(0, -direction * speed * Time.deltaTime, 0);
            if (transform.localPosition.y < minY)
            {
                bounced = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    { // Collision behavior
        if (other.gameObject.CompareTag("Player"))
        { // Destroy when collided with player
            Messenger.Broadcast(GameEvent.HEALTH_PICKUP_COLLECTED);

            Destroy(this.gameObject);
        }
    }
}
