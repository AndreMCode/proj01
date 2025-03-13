using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class WanderingAI : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip fireballLaunch;
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject largeFireballPrefab;
    [SerializeField] Renderer enemyRenderer;
    private Vector3 directionToPlayer;
    private GameObject fireball;
    private Light glow;
    private Ray ray;
    public const float baseSpeed = 6.0f;
    private const float baseGlow = 0.2f;
    private const float obstacleRange = 2f;
    private const float sphereCastRadius = 1.0f;
    private const float angleMin = -110f;
    private const float angleMax = 110f;
    public float speed = baseSpeed;
    private int currentLevel;
    private bool isAlive;
    private bool playerDetected;
    private bool fireballCooldown;
    private bool playerDetectedCooldown;

    void Start()
    {
        isAlive = true;
        playerDetected = false;
        fireballCooldown = false;
        playerDetectedCooldown = false;

        speed *= PlayerPrefs.GetFloat("enemySpeed");
        currentLevel = PlayerPrefs.GetInt("currentLevel");

        // Increase glow from level 1 thru 30
        glow = GetComponent<Light>();
        glow.intensity = Mathf.Clamp(baseGlow * (PlayerPrefs.GetInt("currentLevel") * 1.0f), 0, 6.0f);

        // Increase green tint from level 1 thru 30
        float redBlue = Mathf.Clamp(1.0f / 30.0f * (30 - currentLevel), 0, 1.0f); // 255-- normalized
        Color newColor = new(redBlue, 1.0f, redBlue);
        enemyRenderer.material.SetColor("_Color", newColor);
    }

    void Update()
    {
        if (isAlive)
        {
            // Move forward regardless of turning
            transform.Translate(0, 0, speed * Time.deltaTime);

            // Ray at and pointing forward relative to character
            ray = new Ray(transform.position, transform.forward);
        }

        // In the case of escape artists...
        if (transform.position.x > 25.5f || transform.position.x < -25.5f
        || transform.position.z > 25.5f || transform.position.z < -25.5f)
        {
            isAlive = false;

            Messenger<bool>.Broadcast(GameEvent.RESET_ESCAPEE, true);

            Debug.Log("One got away! D:");
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (playerDetectedCooldown)
        { // One frame delay on detect
            playerDetectedCooldown = false;
            return; // skip raycast
        }

        if (isAlive && !playerDetected)
        {
            int layerMask = ~LayerMask.GetMask("Ignore Raycast");
            // Visualize ray for debugging
            // Debug.DrawRay(ray.origin, ray.direction * 25f, Color.red, 1.0f);

            // Raycast using a sphere, fill the referenced variable (hit) with info
            if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hit, 50.0f, layerMask))
            {
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject.GetComponent<WanderingAI>())
                {
                    return; // skip reaction
                }

                // Player is detected in the same way as the target object in RayShooter
                if (hitObject.GetComponent<PlayerCharacter>() && !fireballCooldown)
                {
                    soundSource.pitch = 1.0f;
                    soundSource.PlayOneShot(fireballLaunch);

                    if (currentLevel >= 31)
                    {
                        fireball = Instantiate(largeFireballPrefab);
                    }
                    else
                    {
                        fireball = Instantiate(fireballPrefab) as GameObject;
                    }
                    // Place fireball in front of enemy and point forward
                    fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.1f); // was 1.5
                    fireball.transform.rotation = transform.rotation;

                    StartCoroutine(FireballCooldown());
                }

                if (!hitObject.GetComponent<PlayerCharacter>() && hit.distance < obstacleRange)
                {
                    // Turn toward a semi-random direction
                    float angle = Random.Range(angleMin, angleMax);
                    transform.Rotate(0, angle, 0);
                }
            }
        }
        else if (isAlive && playerDetected)
        {
            int layerMask = ~LayerMask.GetMask("Ignore Raycast");

            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, 50.0f, layerMask))
            {
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject.GetComponent<WanderingAI>())
                {
                    playerDetected = false;

                    return; // skip reaction
                }

                // Player is detected in the same way as the target object in RayShooter
                if (hitObject.GetComponent<PlayerCharacter>() && !fireballCooldown)
                {
                    // Create quaternion to use basis vector
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    // Lock x and z rotation
                    lookRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
                    // Apply rotation
                    transform.rotation = lookRotation;

                    soundSource.pitch = 1.0f;
                    soundSource.PlayOneShot(fireballLaunch);

                    if (currentLevel >= 31)
                    {
                        fireball = Instantiate(largeFireballPrefab);
                    }
                    else
                    {
                        fireball = Instantiate(fireballPrefab) as GameObject;
                    }
                    // Place fireball in front of enemy and point forward
                    fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.1f); // was 1.5
                    fireball.transform.rotation = transform.rotation;

                    StartCoroutine(FireballCooldown());

                    return;
                }

                if (!hitObject.GetComponent<PlayerCharacter>())
                {
                    playerDetected = false;
                }
            }
        }
    }

    void OnEnable()
    {
        Messenger<float>.AddListener(GameEvent.ENEMY_SPEED_CHANGED, OnSpeedChanged);
        Messenger.AddListener(GameEvent.ENEMY_ACTION_TOGGLE, ToggleAlive);
    }

    void OnDisable()
    {
        Messenger<float>.RemoveListener(GameEvent.ENEMY_SPEED_CHANGED, OnSpeedChanged);
        Messenger.RemoveListener(GameEvent.ENEMY_ACTION_TOGGLE, ToggleAlive);
    }

    private void OnSpeedChanged(float value)
    {
        speed = baseSpeed * value;
    }

    private void ToggleAlive()
    {
        if (isAlive)
        {
            isAlive = false;
        }
        else
        {
            isAlive = true;
        }
    }

    public void SetAlive(bool alive)
    {
        isAlive = alive;
    }

    private IEnumerator FireballCooldown()
    {
        fireballCooldown = true;
        yield return new WaitForSeconds(0.5f);
        fireballCooldown = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            transform.Rotate(0, 180, 0);
        }

        // When player is in proximity of an enemy
        if (other.gameObject.CompareTag("Player") && isAlive)
        {
            // Get player position
            Vector3 playerPosition = other.transform.position;
            // Calculate direction to player and normalize (basis vector)
            directionToPlayer = (playerPosition - transform.position).normalized;

            playerDetected = true;
            playerDetectedCooldown = true;
        }
    }
}
