using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayShooter : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip fxFireWeapon;
    [SerializeField] GameObject fxRicochetPrefab;
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject rifle;

    private GameObject fxRicochet;
    private PistolAction pistolAction;
    private RifleAction rifleAction;
    private Camera cam;
    private Ray pendingRay;
    private bool pendingRaycast = false;
    private bool usingPistol = false;
    private bool usingRifle = false;
    private bool pistolCooldown;
    private bool rifleCooldown;
    public int currentHealth;

    void Start()
    {
        pistolCooldown = false;
        rifleCooldown = false;
        int currentLevel = PlayerPrefs.GetInt("currentLevel", 1);

        if (currentLevel <= 30)
        { // Allow pistol for levels 1 thru 30
            pistol.SetActive(true);
            rifle.SetActive(false);
            usingPistol = true;
            soundSource.pitch = 1.0f;
        }
        else
        { // Allow rifle for levels 31 thru 50
            pistol.SetActive(false);
            rifle.SetActive(true);
            usingRifle = true;
            soundSource.pitch = 0.85f;
        }

        // Give access to other components attached to the same object
        pistolAction = FindObjectOfType<PistolAction>();
        rifleAction = FindObjectOfType<RifleAction>();
        cam = GetComponent<Camera>();

        // Hide the cursor at center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (usingPistol && currentHealth > 0 && !pistolCooldown && Input.GetMouseButtonDown(0) // 0: left mouse button
        && !EventSystem.current.IsPointerOverGameObject()) // Check that click isn't on GUI
        {
            // pixelWidth, pixelHeight of the camera / 2 = center of view
            Vector3 point = new(cam.pixelWidth / 2.0f, cam.pixelHeight / 2.0f, 0);

            // Create ray using calculated point
            pendingRay = cam.ScreenPointToRay(point);
            pendingRaycast = true;
        }

        if (usingRifle && currentHealth > 0 && !rifleCooldown && Input.GetMouseButton(0) // 0: left mouse button held
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
    { // Raycast on physics frame
        if (pendingRaycast)
        {
            PerformRaycast();

            if (usingPistol)
            {
                soundSource.PlayOneShot(fxFireWeapon, 0.85f);
                pistolAction.Action();
            }

            if (usingRifle)
            {
                soundSource.PlayOneShot(fxFireWeapon, 0.75f);
                rifleAction.Action();
            }

            pendingRaycast = false;
        }
    }

    void ProjectileRicochet(Vector3 pos)
    { // Ricochet effect
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

            if (target != null)
            {
                target.ReactToHit();
            }
            else
            {
                // Launch co-routine in response to hit for debugging
                // StartCoroutine(SphereIndicator(hit.point));

                ProjectileRicochet(hit.point);
            }

            // Retrieve coordinates where the ray hit
            // Debug.Log("Hit " + hit.point);
        }
        else
        {
            Debug.Log("Raycast missed!");
        }

        if (usingPistol)
        { // Begin pistol cooldown
            StartCoroutine(PistolCooldown());
        }

        if (usingRifle)
        { // Begin rifle cooldown
            StartCoroutine(RifleCooldown());
        }
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
    { // Track current health to allow or disallow weapon use
        currentHealth = value;
    }

    private IEnumerator PistolCooldown()
    {
        pistolCooldown = true;
        yield return new WaitForSeconds(0.2f);
        pistolCooldown = false;
    }

    private IEnumerator RifleCooldown()
    {
        rifleCooldown = true;
        yield return new WaitForSeconds(0.12f);
        rifleCooldown = false;
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
