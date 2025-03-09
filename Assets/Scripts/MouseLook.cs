using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script contains code used for learning
    purposes. Correctly working final code should
    utilize 'MouseX' for the player object and
    'MouseY' for the camera object!
*/

public class MouseLook : MonoBehaviour
{
    // This associates custom names to numbers
    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    // Create RotationAxes variable 'axes', set to MouseX
    public RotationAxes axes = RotationAxes.MouseX;

    // Horizontal and vertical sensitivity
    public const float baseHorzSens = 1f;
    public const float baseVertSens = 1f;
    public float horzSens;
    public float vertSens;

    // Vertical look limits
    public float vertMin = -60.0f;
    public float vertMax = 60.0f;

    // Vertical angle
    private float vertRot = 0;

    void Start()
    {
        // Prevent physics from affecting playerbody
        if (TryGetComponent<Rigidbody>(out var body))
        {
            body.freezeRotation = true;
        }

        horzSens = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        vertSens = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
    }

    /*
        About 'GetAxis' for mouse input:
        This pulls from a range of
        -1 to 1, depending on glide speed
    */

    void Update()
    {
        // If axes is set to MouseX..
        if (axes == RotationAxes.MouseX)
        {
            // Rotate along Y-axis (horizontal look)
            transform.Rotate(0, Input.GetAxis("Mouse X") * horzSens, 0);
        }
        // Else if axes is set to MouseY..
        else if (axes == RotationAxes.MouseY)
        {
            // Pull vertical glide speed from mouse
            // (Use += for "inverted" vertical look)
            vertRot -= Input.GetAxis("Mouse Y") * vertSens;
            // Limit rotation along X-axis (vertical look)
            vertRot = Mathf.Clamp(vertRot, vertMin, vertMax);

            // Pull existing Y-rotation angle
            float horzRot = transform.localEulerAngles.y;

            // Apply transform
            transform.localEulerAngles = new Vector3(vertRot, horzRot, 0);
        }
        // Else axes must be set to MouseXAndY..
        else
        {
            // Pull vertical glide speed from mouse
            // (Use += for "inverted" vertical look)
            vertRot -= Input.GetAxis("Mouse Y") * vertSens;
            // Limit rotation along X-axis (vertical look)
            vertRot = Mathf.Clamp(vertRot, vertMin, vertMax);

            // Pull horizontal glide speed from mouse
            float delta = Input.GetAxis("Mouse X") * horzSens;
            // Add to existing Y-rotation angle (horizontal look)
            float horzRot = transform.localEulerAngles.y + delta;

            // Apply transform
            transform.localEulerAngles = new Vector3(vertRot, horzRot, 0);
        }
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.MOUSE_SENSITIVITY_CHANGED, SetSensitivity);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.MOUSE_SENSITIVITY_CHANGED, SetSensitivity);
    }

    private void SetSensitivity()
    {
        horzSens = baseHorzSens * PlayerPrefs.GetFloat("MouseSensitivity");
        vertSens = baseVertSens * PlayerPrefs.GetFloat("MouseSensitivity");
    }
}
