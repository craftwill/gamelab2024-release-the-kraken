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

        //Game Events
        public const string TogglePause = "TogglePause";
    }

    public class Vector2EventData : BytesData
    {
        public Vector2EventData(Vector2 vector2) { Vector2 = vector2; }
        public Vector2 Vector2 { get; private set; }
    }
}