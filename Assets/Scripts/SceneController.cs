using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Experimental.GlobalIllumination;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject healthPickupPrefab;
    [SerializeField] Light daylight;
    private FPSInput fpsInput;
    [SerializeField] GameObject extraSpawn1;
    [SerializeField] GameObject extraSpawn2;
    [SerializeField] GameObject extraSpawn3;
    [SerializeField] GameObject extraSpawn4;
    private const float baseHealthSpawn = 15.0f;
    private const float baseDaylight = 1.5f;
    public const int enemiesOnStart = 4;
    private float nextHealthSpawn;
    private float nextHealthTime;
    private bool gamePaused = true;
    private bool spawnPointsRemoved = false;
    private bool spawnOne = false;
    public int currentLevel;
    public int enemyQueue;
    public int enemiesDown;
    public int enemiesRemaining;
    

    void Start()
    {
        fpsInput = FindObjectOfType<FPSInput>();

        currentLevel = PlayerPrefs.GetInt("currentLevel", 1);

        nextHealthTime = 0f;

        enemyQueue = currentLevel;
        enemiesDown = 0;
        enemiesRemaining = enemiesOnStart + enemyQueue;

        // Decrease daylight to zero from level 1 thru 30
        daylight.intensity = Mathf.Clamp(baseDaylight - (0.05f * (currentLevel - 1)), 0, 1.5f);
        Messenger<int>.Broadcast(GameEvent.SET_ENEMIES_REMAINING, enemiesRemaining);

        InitialEnemies();

        // Set level conditions
        if (currentLevel <= 30)
        {
            SpawnAdditional((currentLevel - 1) / 5);

            nextHealthSpawn = baseHealthSpawn;

            extraSpawn1.SetActive(false);
            extraSpawn2.SetActive(false);
            extraSpawn3.SetActive(false);
            extraSpawn4.SetActive(false);
        }
        else if (currentLevel > 30 && currentLevel <= 40)
        {
            SpawnAdditional((currentLevel - 1) / 5);

            // Increased leniency
            nextHealthSpawn = baseHealthSpawn - 5.0f;

            extraSpawn3.SetActive(false);
            extraSpawn4.SetActive(false);
        }
        else
        {
            SpawnAdditional(8); // 12 enemy instances max

            // Default leniency since health pickup heals completely on final levels
            nextHealthSpawn = baseHealthSpawn;
        }
    }

    void Update()
    {
        if (spawnOne && enemyQueue > 0)
        { // Monitor for call to spawn enemy
            Vector3[] spawnPoints;
            if (currentLevel <= 30)
            {
                spawnPoints = new Vector3[]
                {
                    new Vector3(17.0f, 1.01f, 20.0f),
                    new Vector3(-17.0f, 1.01f, -20.0f),
                    new Vector3(-21.0f, 1.01f, 17.0f),
                    new Vector3(21.0f, 1.01f, -17.0f)
                };
            }
            else if (currentLevel > 30 && currentLevel <= 40)
            {
                spawnPoints = new Vector3[]
                {
                    new Vector3(17.0f, 1.01f, 20.0f),
                    new Vector3(-17.0f, 1.01f, -20.0f),
                    new Vector3(-21.0f, 1.01f, 17.0f),
                    new Vector3(21.0f, 1.01f, -17.0f),
                    new Vector3(15.5f, 1.01f, 1f),
                    new Vector3(-15.5f, 1.01f, -1f)
                };
            }
            else
            {
                spawnPoints = new Vector3[]
                {
                    new Vector3(17.0f, 1.01f, 20.0f),
                    new Vector3(-17.0f, 1.01f, -20.0f),
                    new Vector3(-21.0f, 1.01f, 17.0f),
                    new Vector3(21.0f, 1.01f, -17.0f),
                    new Vector3(15.5f, 1.01f, 1f),
                    new Vector3(-15.5f, 1.01f, -1f),
                    new Vector3(0, 1.01f, 9.0f),
                    new Vector3(0, 1.01f, -9.0f)
                };
            }

            // Pick a random spawn point
            Vector3 selectedSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

            float angle = Random.Range(0, 360);

            Instantiate(enemyPrefab, selectedSpawn, Quaternion.Euler(0, angle, 0));

            spawnOne = false;
            enemyQueue--;
        }

        if (!gamePaused)
        { // Monitor time changes before spawning health pickup
            nextHealthTime += Time.deltaTime;

            if (nextHealthTime >= nextHealthSpawn)
            {
                Vector3 position = new(0, 1, 0);
                Instantiate(healthPickupPrefab, position, Quaternion.identity);

                nextHealthTime = -3000.0f;
            }
        }

        if (enemyQueue == 0 && !spawnPointsRemoved)
        { // Stop spawn point effects
            spawnPointsRemoved = true;

            Messenger.Broadcast(GameEvent.ENEMY_QUEUE_DEPLETED);
        }
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.HEALTH_PICKUP_COLLECTED, HealthPickedUp);
        Messenger.AddListener(GameEvent.RESET_GAME, ResetLevel);
        Messenger<bool>.AddListener(GameEvent.RESET_ESCAPEE, ResetEscapee);
        Messenger<float>.AddListener(GameEvent.ENEMY_SPEED_CHANGED, DummyListener1);
        Messenger.AddListener(GameEvent.FIREBALL_PAUSE_TOGGLE, DummyListener2);
        Messenger.AddListener(GameEvent.ENEMY_ACTION_TOGGLE, DummyListener3);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.HEALTH_PICKUP_COLLECTED, HealthPickedUp);
        Messenger.RemoveListener(GameEvent.RESET_GAME, ResetLevel);
        Messenger<bool>.RemoveListener(GameEvent.RESET_ESCAPEE, ResetEscapee);
        Messenger<float>.RemoveListener(GameEvent.ENEMY_SPEED_CHANGED, DummyListener1);
        Messenger.RemoveListener(GameEvent.FIREBALL_PAUSE_TOGGLE, DummyListener2);
        Messenger.RemoveListener(GameEvent.ENEMY_ACTION_TOGGLE, DummyListener3);
    }

    private void HealthPickedUp()
    { // Set health pickup spawn timer
        nextHealthTime = 0f;
    }

    private void ResetEscapee(bool spawn)
    { // NO ESCAPE!
        enemyQueue++;
        spawnOne = spawn;
    }

    private void DummyListener1(float value)
    {
        // Do nothing when no enemies exist
    }

    private void DummyListener2()
    {
        // Do nothing when no fireballs exist except toggle pause flag
        if (gamePaused)
        {
            gamePaused = false;
        }
        else
        {
            gamePaused = true;
        }
    }

    private void DummyListener3()
    {
        // Do nothing when no enemies exist
    }

    private void InitialEnemies()
    { // Initial four enemies
        Vector3[] positions = new Vector3[]
        {
        new Vector3(22.5f, 1.01f, 22.5f),
        new Vector3(22.5f, 1.01f, -22.5f),
        new Vector3(-22.5f, 1.01f, 22.5f),
        new Vector3(-22.5f, 1.01f, -22.5f)
        };

        foreach (Vector3 pos in positions)
        {
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }

    private void SpawnAdditional(int count)
    { // Scatter additional enemies near the bounds of the play area
        Vector3[] spawnPoints = new Vector3[]
        {
        new(10.0f, 1.01f, -19.0f),
        new(-10.0f, 1.01f, 19.0f),
        new(5.0f, 1.01f, -22.5f),
        new(-5.0f, 1.01f, 22.5f),
        new(-22.5f, 1.01f, -2.5f),
        new(22.5f, 1.01f, 2.5f),
        new(0f, 1.01f, 17.0f),
        new(0f, 1.01f, -17.0f),
        new(-5.0f, 1.01f, -20.0f),
        new(5.0f, 1.01f, 20.0f),
        new(21.5f, 1.01f, -7.0f),
        new(-21.5f, 1.01f, 7.0f)
        };

        for (int i = 0; i < count; i++)
        {
            Vector3 selectedSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

            float angle = Random.Range(0, 360);

            Instantiate(enemyPrefab, selectedSpawn, Quaternion.Euler(0, angle, 0));

            enemyQueue--;
        }
    }

    public void SpawnOne(bool spawn)
    {
        spawnOne = spawn;
    }

    public void EnemyDown()
    { // Decrement enemy count
        enemiesDown++;
        enemiesRemaining--;

        if (enemiesRemaining == 0)
        { // Level clear logic
            Messenger.Broadcast(GameEvent.LEVEL_COMPLETE);

            // Update record highest level
            if (PlayerPrefs.GetInt("currentLevel") > PlayerPrefs.GetInt("highestLevel"))
            {
                PlayerPrefs.SetInt("highestLevel", PlayerPrefs.GetInt("currentLevel"));
                Debug.Log("HighestLevel updated");
            }

            // Set checkpoint level: 1, 6, 11.. ..46
            if (PlayerPrefs.GetInt("currentLevel") % 5 == 0 && currentLevel < 50)
            {
                PlayerPrefs.SetInt("retryLevel", PlayerPrefs.GetInt("currentLevel") + 1);
                PlayerPrefs.SetFloat("retryEnemySpeed", PlayerPrefs.GetFloat("enemySpeed") + 0.01f);
                Debug.Log("Level checkpoint set");
            }

            // Validate level change
            PlayerPrefs.SetInt("validLevel", 1);

            if (currentLevel < 50)
            {
                // Iterate level and enemy speed
                PlayerPrefs.SetInt("currentLevel", PlayerPrefs.GetInt("currentLevel", 1) + 1);
                PlayerPrefs.SetFloat("enemySpeed", PlayerPrefs.GetFloat("enemySpeed", 1) + 0.01f);
            }
            else
            {
                // Game complete sequence..
            }

            fpsInput.enabled = false;
            StartCoroutine(ResetGame());
            StartCoroutine(GetReadyText());

            BGMManager bgmManager = FindObjectOfType<BGMManager>();
            bgmManager.OnPlayerWin();
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator GetReadyText()
    { // Prepare for proceeding level sequence
        yield return new WaitForSeconds(3.0f);

        Messenger.Broadcast(GameEvent.GET_READY);
        yield return new WaitForSeconds(0.4f);
        Messenger.Broadcast(GameEvent.GET_READY);
        yield return new WaitForSeconds(0.1f);

        Messenger.Broadcast(GameEvent.GET_READY);
        yield return new WaitForSeconds(0.4f);
        Messenger.Broadcast(GameEvent.GET_READY);
        yield return new WaitForSeconds(0.1f);

        Messenger.Broadcast(GameEvent.GET_READY);
        yield return new WaitForSeconds(0.4f);
        Messenger.Broadcast(GameEvent.GET_READY);
        yield return new WaitForSeconds(0.1f);

        Messenger.Broadcast(GameEvent.GET_READY);
    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(5.0f);

        ResetLevel();
    }
}

/* - To prevent enemy spawning near player, kind of encourages camping though
void Update()
    {
        if (spawnOne && enemyQueue > 0)
        {
            // Define spawn points
            Vector3[] spawnPoints = new Vector3[]
            {
            new Vector3(22.5f, 1.01f, 22.5f),
            new Vector3(-22.5f, 1.01f, -22.5f),
            new Vector3(-22.5f, 1.01f, 13f),
            new Vector3(22.5f, 1.01f, -13f)
            };

            // Get the player's position
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.LogWarning("Player object not found!");
                return;
            }
            Vector3 playerPosition = player.transform.position;

            // Find the closest spawn point to the player
            Vector3 closestSpawn = spawnPoints.OrderBy(spawn => Vector3.Distance(spawn, playerPosition)).First();

            // Filter out the closest spawn point
            Vector3[] validSpawns = spawnPoints.Where(spawn => spawn != closestSpawn).ToArray();

            // Pick a random valid spawn point
            Vector3 selectedSpawn = validSpawns[Random.Range(0, validSpawns.Length)];

            float angle = Random.Range(0, 360);

            Instantiate(enemyPrefab, selectedSpawn, Quaternion.Euler(0, angle, 0));

            spawnOne = false;
            enemyQueue--;
        }
    }
*/