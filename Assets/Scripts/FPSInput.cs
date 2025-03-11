using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Experimental;

// To Ensure CharacterController component is attached
[RequireComponent(typeof(CharacterController))]
// Adds this script to the "Add Component" drop menu in Unity
[AddComponentMenu("Control Script/FPS Input")]

public class FPSInput : MonoBehaviour
{
    [SerializeField] MouseLook mouseLookX;
    [SerializeField] MouseLook mouseLookY;
    [SerializeField] RayShooter rayShooter;
    private CharacterController charController;
    public const float baseSpeed = 8.0f;
    public float speed = baseSpeed;
    public float enemySpeed; // temporarily hold enemy speed during game pause
    public bool gamePaused = false;
    public bool popupOpen = false;
    public float speedFactor;
    public float gravity = -9.8f;

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        { // Pause function on Tab key
            if (gamePaused && !popupOpen)
            {
                ExitPopupMenu();
            }
            else if (!popupOpen)
            {
                gamePaused = true;
                Messenger.Broadcast(GameEvent.ENEMY_ACTION_TOGGLE);
                PlayerPrefs.SetInt("gamePaused", 1);

                enemySpeed = PlayerPrefs.GetFloat("enemySpeed", 1.0f);
                PlayerPrefs.SetFloat("previousEnemySpeed", PlayerPrefs.GetFloat("enemySpeed", 1.0f)); // Probably useless now
                PlayerPrefs.SetFloat("enemySpeed", 0f);

                Messenger.Broadcast(GameEvent.PAUSE_GLYPH_OFF);
                Messenger.Broadcast(GameEvent.PAUSE_INDICATOR_ON);
                Messenger.Broadcast(GameEvent.CROSSHAIR_OFF);
                Messenger.Broadcast(GameEvent.SETTINGS_BUTTON_TOGGLE);
                Messenger.Broadcast(GameEvent.FIREBALL_PAUSE_TOGGLE);
                Messenger<float>.Broadcast(GameEvent.PLAYER_SPEED_CHANGED, 0f);
                Messenger<float>.Broadcast(GameEvent.ENEMY_SPEED_CHANGED, 0f);
                mouseLookX.horzSens = 0;
                mouseLookY.vertSens = 0;
                rayShooter.enabled = false; //
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        // 'Horizontal' pulls A and D, left and right, from keyboard
        float deltaX = Input.GetAxis("Horizontal") * speed;
        // 'Vertical' pulls W and S, up and down, from keyboard
        float deltaZ = Input.GetAxis("Vertical") * speed;

        // Apply movement magnitude to vector
        Vector3 movement = new(deltaX, 0, deltaZ);

        /*
            Limit the vector's magnitude to the movement speed
            (preventing, for ex., both up and left simultaneously
            from exceeding the intended move speed)
        */
        movement = Vector3.ClampMagnitude(movement, speed);

        // Allow gravity constant to affect Y-axis of player
        movement.y = gravity;

        // Calculate movement independent of refresh rate
        movement *= Time.deltaTime;

        // Convert local space vector movement to global space vector
        movement = transform.TransformDirection(movement);

        // Apply movement
        charController.Move(movement);
    }

    void OnEnable()
    {
        Messenger<float>.AddListener(GameEvent.PLAYER_SPEED_CHANGED, OnSpeedChanged);
        Messenger.AddListener(GameEvent.POPUP_IS_OPEN, IsPopup);
        Messenger.AddListener(GameEvent.EXIT_POPUP_MENU, ExitPopupMenu);
    }

    void OnDisable()
    {
        Messenger<float>.RemoveListener(GameEvent.PLAYER_SPEED_CHANGED, OnSpeedChanged);
        Messenger.RemoveListener(GameEvent.POPUP_IS_OPEN, IsPopup);
        Messenger.RemoveListener(GameEvent.EXIT_POPUP_MENU, ExitPopupMenu);
    }

    private void OnSpeedChanged(float value)
    { // Set speed when paused
        speed = baseSpeed * value;
    }

    private void IsPopup()
    { // Settings open check
        popupOpen = true;
    }

    private void ExitPopupMenu()
    { // Unpause game
        gamePaused = false;
        Messenger.Broadcast(GameEvent.ENEMY_ACTION_TOGGLE);
        PlayerPrefs.SetInt("gamePaused", -1);

        if (enemySpeed == 0)
        {
            enemySpeed = PlayerPrefs.GetFloat("enemySpeed");
        }
        else
        {
            PlayerPrefs.SetFloat("enemySpeed", enemySpeed);
        }

        Messenger.Broadcast(GameEvent.PAUSE_GLYPH_ON);
        Messenger.Broadcast(GameEvent.PAUSE_INDICATOR_OFF);
        Messenger.Broadcast(GameEvent.CROSSHAIR_ON);
        Messenger.Broadcast(GameEvent.SETTINGS_BUTTON_TOGGLE);
        Messenger.Broadcast(GameEvent.FIREBALL_PAUSE_TOGGLE);
        Messenger<float>.Broadcast(GameEvent.PLAYER_SPEED_CHANGED, 1.0f);
        Messenger<float>.Broadcast(GameEvent.ENEMY_SPEED_CHANGED, enemySpeed);
        Messenger.Broadcast(GameEvent.MOUSE_SENSITIVITY_CHANGED);
        rayShooter.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        popupOpen = false;
    }
}
