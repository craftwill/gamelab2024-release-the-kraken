using System;
using UnityEngine;
using UnityEngine.Events;

using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class EventNames
    {
        // Custom late start()
        public const string LateStart = "LateStart";

        // Main menu
        public const string TryConnectToPhoton = "TryConnectToPhoton";
        public const string ConnectedToMaster = "ConnectedToMaster";
        public const string NetworkError = "NetworkError";

        // Lobby Events
        public const string JoinRoomWithCode = "JoinRoomWithCode";
        public const string CreateRoom = "CreateRoom";
        public const string UpdateLobbyView = "UpdateLobbyView";
        public const string JoinGameScene = "JoinGameScene";

        // GameFlow events
        public const string StartGameFlow = "StartGameFlow";
        public const string StopGameFlow = "StopGameFlow";
        public const string StartGameTimer = "StartGameTimer";
        public const string LeaveGame = "LeaveGame";

        // Objectives events
        public const string StartObjectives = "StartObjectives";
        public const string NextObjective = "NextObjective";
        public const string StopObjectives = "StopObjectives";
        public const string ZoneFullLoss = "ZoneFullLoss";

        // Spawning
        public const string StartSpawning = "StartSpawning";
        public const string StopSpawning = "StopSpawning";

        // UI events
        public const string UpdateCountownTimerUI = "UpdateCountownTimerUI";
        public const string UpdateObjectiveUI = "UpdateObjectiveUI";
        public const string UpdateObjectiveTimerUI = "UpdateObjectiveTimerUI";
        public const string UpdateGameTimerUI = "UpdateGameTimerUI";
        public const string ShowVictoryScreenUI = "ShowVictoryScreenUI";
        public const string ShowDefeatScreenUI = "ShowDefeatScreenUI";
        public const string UpdateUltimateUI = "UpdateUltimateUI";
        public const string UpdateHealthUI = "UpdateHealthUI";
        public const string UpdateCurrentZoneOccupancyUI = "UpdateCurrentZoneOccupancyUI";

        //Game Events
        public const string TogglePause = "TogglePause";
        public const string GainWool = "GainWool";
        public const string UpdateWoolQuantity = "UpdateWoolQuantity";

        //Player Events
        public const string PlayerDeath = "PlayerDeath";
        public const string PlayerAttackStart = "PlayerAttackStart";
        public const string PlayerAttackEnd = "PlayerAttackEnd";
        public const string PlayerWin = "PlayerWin";
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
}