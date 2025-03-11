using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public const float baseSpeed = 24.0f;
    public float speed;
    public int damage = 1;

    void Start()
    { // Set fireball speed
        speed = baseSpeed;
    }
    void Update()
    { // Translate fireball
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.FIREBALL_PAUSE_TOGGLE, ToggleSpeed);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.FIREBALL_PAUSE_TOGGLE, ToggleSpeed);
    }

    private void ToggleSpeed()
    { // Toggle translation when game paused
        if (speed == baseSpeed)
        {
            speed = 0f;
        }
        else
        {
            speed = baseSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    { // Collision behavior
        if (!other.gameObject.CompareTag("Health Pickup"))
        { // Fireball ignores health pickups and acts if collided with player
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();

            if (player != null)
            {
                player.Hurt(damage);
            }

            Destroy(this.gameObject);
        }
    }
}