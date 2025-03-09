using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayShooter : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip fxFireWeapon;
    [SerializeField] GameObject fxRicochetPrefab;

    private GameObject fxRicochet;
    private PistolAction pistolAction;
    private Camera cam;
    private Ray pendingRay;
    private bool pendingRaycast = false;
    private bool pistolCooldown;
    public int currentHealth;

    void Start()
    {
        pistolCooldown = false;
        // Give access to other components attached to the same object
        pistolAction = FindObjectOfType<PistolAction>();
        cam = GetComponent<Camera>();

        // Hide the cursor at center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (currentHealth > 0 && !pistolCooldown && Input.GetMouseButtonDown(0) // 0: left mouse button
        && !EventSystem.current.IsPointerOverGameObject()) // Check that click isn't on GUI
        {
            // pixelWidth, pixelHeight of the camera / 2 = center of view
            Vector3 point = new(cam.pixelWidth / 2.0f, cam.pixelHeight / 2.0f, 0);

            // Create ray using calculated point
            pendingRay = cam.ScreenPointToRay(point);
            pendingRaycast = true;
        }
    }

    void FixedUpdate()
    {
        if (pendingRaycast)
        {
            soundSource.PlayOneShot(fxFireWeapon);
            PerformRaycast();
            pistolAction.Action();
            pendingRaycast = false;
        }
    }

    void ProjectileRicochet(Vector3 pos)
    {
        fxRicochet = Instantiate(fxRicochetPrefab);
        fxRicochet.transform.position = pos;
    }

    void PerformRaycast()
    {
        int layerMask = ~LayerMask.GetMask("Enemy-Player Detection", "Ignore Raycast");
        // Visualize ray for debugging
        // Debug.DrawRay(pendingRay.origin, pendingRay.direction * 25f, Color.red, 1.0f);

        // Raycast using a ray, fill the referenced variable (hit) with info
        if (Physics.Raycast(pendingRay, out RaycastHit hit, 100f, layerMask))
        {
            // Retrieve transform of the object hit by ray
            GameObject hitObject = hit.transform.gameObject;
            ReactiveTarget target = hitObject.GetComponent<ReactiveTarget>();
            ReactiveTarget2 target2 = hitObject.GetComponent<ReactiveTarget2>();

            if (target != null || target2 != null)
            {
                if (target != null)
                {
                    target.ReactToHit();
                }

                if (target2 != null)
                {
                    target2.ReactToHit();
                }
            }
            else
            {
                // Launch co-routine in response to hit for debugging
                // StartCoroutine(SphereIndicator(hit.point));

                // ricochet effect
                ProjectileRicochet(hit.point);
            }

            // Retrieve coordinates where the ray hit
            // Debug.Log("Hit " + hit.point);
        }
        else
        {
            Debug.Log("Raycast missed!");
        }
        
        StartCoroutine(PistolCooldown());
    }

    void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.HEALTH_REMAINING, HealthRemaining);
    }

    void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.HEALTH_REMAINING, HealthRemaining);
    }

    private void HealthRemaining(int value)
    {
        currentHealth = value;
    }

    private IEnumerator PistolCooldown()
    {
        pistolCooldown = true;
        yield return new WaitForSeconds(0.1f);
        pistolCooldown = false;
    }

    // private IEnumerator SphereIndicator(Vector3 pos)
    // {
    //     GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //     sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    //     sphere.transform.position = pos;

    //     // Tells the co-routine where to pause and how long
    //     yield return new WaitForSeconds(1);

    //     Destroy(sphere); // Remove and clear from memory
    // }
}
