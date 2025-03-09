using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    // Game
    public const string SET_ENEMIES_REMAINING = "SET_ENEMIES_REMAINING";
    public const string ENEMIES_REMAINING = "ENEMIES_REMAINING";
    public const string HEALTH_REMAINING = "HEALTH_REMAINING";
    public const string ENEMY_SPEED_CHANGED = "ENEMY_SPEED_CHANGED";
    public const string PLAYER_SPEED_CHANGED = "PLAYER_SPEED_CHANGED";
    public const string HEALTH_PICKUP_COLLECTED = "HEALTH_PICKUP_COLLECTED";
    public const string MOUSE_SENSITIVITY_CHANGED = "MOUSE_SENSITIVITY_CHANGED";
    public const string ENEMY_ACTION_TOGGLE = "ENEMY_ACTION_TOGGLE";
    public const string FIREBALL_PAUSE_TOGGLE = "FIREBALL_PAUSE_TOGGLE";
    public const string RESET_GAME = "RESET_GAME";
    public const string RESET_ESCAPEE = "RESET_ESCAPEE";
    // UI
    public const string CROSSHAIR_ON = "CROSSHAIR_ON";
    public const string CROSSHAIR_OFF = "CROSSHAIR_OFF";
    public const string POPUP_IS_OPEN = "POPUP_IS_OPEN";
    public const string PAUSE_INDICATOR_ON = "PAUSE_INDICATOR_ON";
    public const string PAUSE_INDICATOR_OFF = "PAUSE_INDICATOR_OFF";
    public const string PAUSE_GLYPH_ON = "PAUSE_GLYPH_ON";
    public const string PAUSE_GLYPH_OFF = "PAUSE_GLYPH_OFF";
    public const string SETTINGS_BUTTON_TOGGLE = "SETTINGS_BUTTON_TOGGLE";
    public const string EXIT_POPUP_MENU = "EXIT_POPUP_MENU";
    public const string LEVEL_COMPLETE = "LEVEL_COMPLETE";
    public const string GET_READY = "GET_READY";
    public const string GAME_OVER = "GAME_OVER";
}
