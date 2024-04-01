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
        [Tooltip("Randomize objectives")]
        public bool randomizeObjectives = true;
        public int spawnObjectiveCount = 3;
        public int minibossObjectiveCount = 2;
        [Tooltip("Maximum time the game can last in seconds")]
        public int gameDuration = 600;

        [Header("Player settings")]
        public int maxHealth = 100;
        [Header("Razzle")]
        public float razzleAbilityCooldown = 10f;
        public float razzleAbilityPullDuration = 5f;
        public float razzleAbilityRadius = 4f;
        public float razzleAbilityInitialPullStrength = 4f;
        public float razzleAbilityPullStrengthOverTime = 1.5f;
        public float razzleAbilitySpawnDistanceOffset = 5f;
        public Vector3 razzleSpawnPoint = Vector3.zero;
        [Header("Dazzle")]
        public float dazzleAbilityCooldown = 10f;
        public float dazzleAbilityJumpDuration = 0.27f;
        public float dazzleAbilityJumpDistance = 7f;
        public float dazzleAbilityJumpHeigth = 3f;
        public float dazzleAbilityDamage = 5f;
        public float dazzleAbilityRadius = 3f;
        public Vector3 dazzleSpawnPoint = Vector3.zero;

        [Header("Enemy settings")]
        public float enemyMinRoamDistance = 4f;
        public float enemyMaxRoamDistance = 10f;
        public float enemyRoamMinChangeFrequency = 2f;
        public float enemyRoamMaxChangeFrequency = 5f;

        [Header("Player Controls settings")]
        public float moveSpeed = 7.5f;
        public float sprintSpeed = 14f;
        public float attackMoveSpeed = 10f;
        public float rotationSpeed = 50f;
        public float gravity = 5f;
        public float dashDuration = 0.2f;
        public float dashSpeed = 30f;
        public float dashCooldown = 3.0f;
        public float sprintAfterAttackCooldown = 0.4f;

        [Header("Camera settings")]
        public float cameraSensitivity = 2f;
        public float yCameraSensitivityMultiplier = 0.25f;
        public float cameraControllerMultiplier = 10f;
        public bool invertXAxis = false;
        public bool invertYAxis = false;
        public float baseFov = 50f;
        public float sprintFov = 60f;
        public float fovChangeDuration = 0.2f;
        public bool changeFovOnSprint = true;

        [Header("Duo Ultimate Attack")]
        public float ultimateStartMaxDistance = 5f;
        public float ultimateEndDistance = 3f;
        public float ultimateMinDamageDistance = 15f;
        public int ultimateDamage = 10;
        public int ultimateMinDamage = 3;
        public float ultimateDuration = 5f;
        public float ultimateTriggerTimer = 5f;
        public float ultimateCooldown = 10f;
        public int ultimateMinWool = 10;
        public float ultimateDistancePerWool = 1f;
        public float ultimateMinimumDuration = 0.5f;
        public bool ultimateDoesSlowMo = true;
        public float ultimateSlowMoTimeScale = 0.5f;
        public float ultimateSlowMoDuration = 1f;
        public bool ultimateIsCancellable = false;

        [Header("Tower settigns")]
        public int towerWoolCost = 50;
        public int maxTowerPerRound = 2;
        [Tooltip("A zone with a tower in it will spawn x timer slower")] public float towerSpawnMultiplier = 5;

        [Header("Healing settings")]
        [Tooltip("The max distance between the two players to be able to heal")]
        public float healingMaxDistance = 3f;
        [Tooltip("Heals 1 HP every x seconds")]
        public float healingRate = 0.1f;
        [Tooltip("How much hp one wool lets you heal")]
        public float healingHpPerWool = 10f;

        [Header("Combat Settings")]
        public float enemyStaggerDuration = 0.5f;
        public float comboPitchIncrement = 100f;
        public float comboMaxHitInterval = 0.5f;
        public float hitFlashDuration = 0.1f;
        public float knockbackForce = 25f;
        public float knockBackDuration = 0.1f;

        [Header("Minimap Settings")]
        public bool showBasicEnemies = false;

        [Header("LilWool Settings")]
        public float minWoolVerticalVelocity = 1f;
        public float maxWoolVerticalVelocity = 2f;
        public float maxWoolHorizontalVelocity = 1f;
        public float maxWoolAngularVelocity = 5f;
        public float woolRagdollDuration = 2f;
        public float woolAcceleration = 10f;
        public float woolDrag = 5f;
        public int initialWoolQuantity = 0;
        public int maxWoolQuantity = 50;

        [Header("UI Settings")]
        public bool hideGameCanvasOnPause = false;

        [Header("Music Settings")]
        public bool useObjectiveMusic = true;

        [Header("Scaling Settings")]
        [Tooltip("Multiplies a delay, should be < 1")]
        public float permanentSpawningScaling = 0.8f;
        [Tooltip("Multiplies a delay, should be < 1")]
        public float objectiveSpawningScaling = 1f;
        [Tooltip("Multiplies a value, should be > 1")]
        public float enemyHealthScaling = 1f;
    }
}