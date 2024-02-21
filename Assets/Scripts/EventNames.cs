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

        // Objectives events
        public const string StartObjectives = "StartObjectives";
        public const string NextObjective = "NextObjective";
        public const string StopObjectives = "StopObjectives";

        // Spawning
        public const string StartSpawning = "StartSpawning";
        public const string StopSpawning = "StopSpawning";

        // UI events
        public const string UpdateCountownTimerUI = "UpdateCountownTimerUI";
        public const string UpdateObjectiveUI = "UpdateObjectiveUI";
        public const string UpdateGameTimerUI = "UpdateGameTimerUI";
        
        //Game Events
        public const string TogglePause = "TogglePause";
        public const string ToggleCursor = "ToggleCursor";

        //Player Events
        public const string PlayerDeath = "PlayerDeath";
        public const string PlayerAttackStart = "PlayerAttackStart";
        public const string PlayerAttackEnd = "PlayerAttackEnd";
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
        public UpdateObjectiveUIData(string objectiveName) { ObjectiveName = objectiveName; }
        public string ObjectiveName { get; private set; }
    }
}