using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "GlobalConfig", menuName = "Kraken/Create Global Config")]
    public class ConfigSO : ScriptableObject
    {
        [Header("Testing")]
        public bool showDebugLogs;
        public bool requireTwoPlayers = true;
        public bool forceUseDazzlePlayer1 = false;
        public bool requireTwoPlayersForUltimate = true;

        [Header("Network")]
        public string loadCustomSceneName = "";
        public bool isSkipMainMenu = false;

        [Header("GameFlow")]
        [Tooltip("Skip need to wait for two players to have joined.")]
        public bool useDebugGameFlow = true;
        [Tooltip("Maximum time the game can last in seconds")]
        public int gameDuration = 600;

        [Header("Player Controls settings")]
        public float moveSpeed = 7.5f;
        public float sprintSpeed = 14f;
        public float attackMoveSpeed = 10f;
        public float rotationSpeed = 50f;
        public float gravity = 5f;
        public float cameraSensitivity = 2f;
        public float yCameraSensitivityMultiplier = 0.25f;
        public float cameraControllerMultiplier = 10f;
        public bool invertXAxis = false;
        public bool invertYAxis = false;
        public float dashDuration = 0.2f;
        public float dashSpeed = 30f;
        public float dashCooldown = 3.0f;

        [Header("Duo Ultimate Attack")]
        public float ultimateStartMaxDistance = 5f;
        public float ultimateEndDistance = 3f;
        public float ultimateMinDamageDistance = 15f;
        public int ultimateDamage = 10;
        public int ultimateMinDamage = 3;
        public float ultimateDuration = 5f;
        public float ultimateTriggerTimer = 5f;
        public float ultimateCooldown = 10f;
    }
}