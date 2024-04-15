
using UnityEngine;

using Bytes;

namespace Kraken
{
    public class EventNames
    {
        // Custom late start()
        public const string LateStart = "LateStart";

        // Input events
        public const string InputSchemeChanged = "InputSchemeChanged";

        // Main menu
        public const string TryConnectToPhoton = "TryConnectToPhoton";
        public const string ConnectedToMaster = "ConnectedToMaster";
        public const string NetworkError = "NetworkError";
        public const string JoinFailed = "JoinFailed";

        // Lobby Events
        public const string JoinRoomWithCode = "JoinRoomWithCode";
        public const string CreateRoom = "CreateRoom";
        public const string UpdateLobbyView = "UpdateLobbyView";
        public const string JoinGameScene = "JoinGameScene";

        //Settings Events
        public const string UpdateCameraSettings = "UpdateCameraSettings";

        // GameFlow events
        public const string StartGameFlow = "StartGameFlow";
        public const string StopGameFlow = "StopGameFlow";
        public const string StartGameTimer = "StartGameTimer";
        public const string LeaveGame = "LeaveGame";
        public const string EnterMenu = "EnterMenu";
        public const string LeaveLobby = "LeaveLobby";

        // Objectives events
        public const string StartObjectives = "StartObjectives";
        public const string NextObjective = "NextObjective";
        public const string StopObjectives = "StopObjectives";
        public const string ZoneFullLoss = "ZoneFullLoss";
        public const string MinibossCountChange = "MinibossDeath";
        public const string BossSpawned = "BossSpawned";
        public const string PlayerEnteredObjective = "PlayerEnteredObjective";
        public const string PlayerLeftObjective = "PlayerLeftObjective";

        // Spawning
        public const string StartSpawning = "StartSpawning";
        public const string StopSpawning = "StopSpawning";

        // UI events
        public const string SetupHUD = "SetupHUD";
        public const string InitTimeLeftUI = "InitTimeLeftUI";
        public const string ShowDefeatTimeLeftUI = "ShowDefeatTimeLeftUI";
        public const string HideHUD = "HideHUD";
        public const string UpdateCountownTimerUI = "UpdateCountownTimerUI";
        public const string UpdateObjectiveUI = "UpdateObjectiveUI";
        public const string UpdateObjectiveTimerUI = "UpdateObjectiveTimerUI";
        public const string UpdateGameTimerUI = "UpdateGameTimerUI";
        public const string ShowVictoryScreenUI = "ShowVictoryScreenUI";
        public const string ShowDefeatScreenUI = "ShowDefeatScreenUI";
        public const string UpdateBossHealthUI = "UpdateBossHealthUI";
        public const string UpdateCurrentZoneOccupancyUI = "UpdateCurrentZoneOccupancyUI";
        public const string ShowReinforcementHintUI = "ShowReinforcementHintUI";
        public const string ShowDefeatByPlayerUI = "ShowDefeatByPlayerUI";
        public const string ShowDefeatByZoneUI = "ShowDefeatByZoneUI";
        public const string UpdatePatchingUpUI = "UpdatePatchingUpUI";
        public const string UpdateUIScale = "UpdateUIScale";

        // Player Profile HUD events
        public const string UpdatePlayerHealthUI = "UpdateHealthUI";
        public const string UpdateOtherPlayerHealthUI = "UpdateOtherPlayerHealthUI";

        // Player Abilities HUD events
        public const string UpdateUltimateUI = "UpdateUltimateUI";
        public const string StartAbilityCooldown = "StartAbilityCooldown";
        public const string UpdatePlayerHealAbilityUI = "UpdatePlayerHealAbilityUI";
        public const string UpdatePlayerTotemAbilityUI = "UpdatePlayerTotemAbilityUI";
        
        //Game Events
        public const string TogglePause = "TogglePause";
        public const string GainWool = "GainWool";
        public const string UpdateWoolQuantity = "UpdateWoolQuantity";
        public const string UltimateRunning = "UltimateRunning";
        public const string IncreaseCombo = "IncreaseCombo";

        //Player Events
        public const string PlayerDeath = "PlayerDeath";
        public const string PlayerAttackStart = "PlayerAttackStart";
        public const string PlayerAttackEnd = "PlayerAttackEnd";
        public const string PlayerWin = "PlayerWin";
        public const string PlayerPatchingUp = "PlayerPatchingUp";

        //Tower Events
        public const string TowerAttemptBuilt = "TowerAttemptBuilt";
        public const string TowerCancelBuilt = "TowerCancelBuilt";
        public const string TowerBuilt = "TowerBuilt";
        public const string PlayerEnteredTower = "PlayerEnteredTower";
        public const string PlayerLeftTower = "PlayerLeftTower";
    }

    public class ZoneEventData : BytesData
    {
        public ZoneEventData(Zone z) { Zone = z; }
        public Zone Zone { get; private set; }
    }

    public class Vector2EventData : BytesData
    {
        public Vector2EventData(Vector2 vector2) { Vector2 = vector2; }
        public Vector2 Vector2 { get; private set; }
    }

    // UI 
    public class UpdateCountownTimerUIData : BytesData
    {
        public UpdateCountownTimerUIData(float countdownTime, System.Action endCallback) { CountdownTime = countdownTime; EndCallback = endCallback; }
        public float CountdownTime { get; private set; }
        public System.Action EndCallback { get; private set; }
    }

    public class UpdateObjectiveUIData : BytesData
    {
        public UpdateObjectiveUIData(string objectiveName, int objectiveTimer)
        {
            ObjectiveName = objectiveName;
            ObjectiveTimer = objectiveTimer;
        }
        public string ObjectiveName { get; private set; }
        public int ObjectiveTimer { get; private set; }
    }

    public class UpdateZoneOccupancyUIData : BytesData
    {
        public UpdateZoneOccupancyUIData(int enemyCount, int maxEnemyCount)
        {
            EnemyCount = enemyCount;
            MaxEnemyCount = maxEnemyCount;
        }
        public int EnemyCount { get; private set; }
        public int MaxEnemyCount { get; private set; }
    }

    public class SetupHUDData : BytesData
    {
        public SetupHUDData(bool isRazzle, bool isKeyboard)
        {
            IsRazzle = isRazzle;
            IsKeyboard = isKeyboard;
        }
        public bool IsRazzle { get; private set; }
        public bool IsKeyboard { get; private set; }
    }

    public class UpdateBossHealthUIData : BytesData
    {
        public UpdateBossHealthUIData(bool isMiniBoss, float hpAmount, bool isShowUltIndicator = false)
        {
            IsMiniBoss = isMiniBoss;
            HpFillAmount = hpAmount;
            IsShowUltIndicator = isShowUltIndicator;
        }
        public bool IsMiniBoss { get; private set; }
        public float HpFillAmount { get; private set; }
        public bool IsShowUltIndicator { get; private set; }
    }
}