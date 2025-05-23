﻿using System.Collections;
using UnityEngine;

namespace Kraken
{
    public class Config
    {
        public static ConfigSO current;
        
        public static float MasterVolumeMultiplier { get; set; } = 1f;
        public static float SoundVolumeMultiplier { get; set; } = 1f;
        public static float MusicVolumeMultiplier { get; set; } = 0.4f;
        
        // Key names for player settings and preferences saving
        public const string PLAYER_NAME_KEY = "NickName";
        public const string MASTER_VOLUME_KEY = "MasterVolume";
        public const string MUSIC_VOLUME_KEY = "MusicVolume";
        public const string SFX_VOLUME_KEY = "SFXVolume";
        public const string UI_VOLUME_KEY = "UIVolume";
        public const string CONTROLS_KEY = "Controls";

        // Game keys
        public const string GAME_NIGHT_KEY = "GameNight";
        public const string GAME_MAX_NIGHT_KEY = "MaxGameNight";

        // Settings keys
        public const string DISPLAY_FULLSCREEN = "DisplayFullscreen";
        public const string CAMERA_INVERT_Y_AXIS = "CameraInvertYAxis";
        public const string CAMERA_INVERT_X_AXIS = "CameraInvertXAxis";
        public const string CAMERA_SENSITIVITY = "CameraSensitivity";
        public const string VOLUME_MUSIC = "VolumeMusic";
        public const string VOLUME_SOUNDS = "VolumeSounds";
        public const string UI_SCALE = "UIScale";

        // Wwise RTPC names
        public const string COMBO_PITCH = "ComboPitch";
    }
}