using System;
using UnityEngine;
using UnityEngine.Events;

using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class EventNames
    {
        static public string InitNetwork = "InitNetwork";
        static public string StartGame = "StartGame";
        static public string CreateRoom = "CreateRoom";
        static public string JoinRoomWithCode = "JoinRoomWithCode";
        static public string JoinGameScene = "JoinGameScene";

        static public string SetLocalPlayerNickName = "SetLocalPlayerNickName";
    }

    public class Vector2EventData : BytesData
    {
        public Vector2EventData(Vector2 vector2) { Vector2 = vector2; }
        public Vector2 Vector2 { get; private set; }
    }
}