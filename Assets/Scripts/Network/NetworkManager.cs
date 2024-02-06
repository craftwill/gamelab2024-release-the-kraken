using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

using Bytes;

namespace Kraken
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        const int MAX_LOBBY_SIZE = 3;
        const string TEST_ROOM_CODE = "test";

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            EventManager.AddEventListener(EventNames.TryConnectToPhoton, HandleTryConnectToPhoton);
        }

        private void HandleTryConnectToPhoton(BytesData data)
        {
            TryConnectToPhoton();
        }

        public void TryConnectToPhoton()
        {
            TryConnect();
            PhotonNetwork.AutomaticallySyncScene = true;

            EventManager.AddEventListener(EventNames.CreateRoom, HandleCreateRoom);
            EventManager.AddEventListener(EventNames.JoinRoomWithCode, HandleJoinRoom);
        }

        private void TryConnect()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Logged in the master server in " + PhotonNetwork.CloudRegion + " region!");
            PhotonNetwork.AutomaticallySyncScene = true;

            EventManager.Dispatch(EventNames.ConnectedToMaster, null);

            PhotonNetwork.LocalPlayer.NickName = "Player" + UnityEngine.Random.Range(1, 100);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            EventManager.Dispatch(EventNames.NetworkError, null);
        }

        /// <summary>
        /// On player joined room callback function
        /// </summary>
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            Debug.Log("On Joined Room!");
            if (!PhotonNetwork.IsMasterClient) return;

            // Load lobby view
            PhotonNetwork.LoadLevel("Lobby");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            // If played failed to create the TEST_ROOM_CODE room, then it must already exist. Join it instead. 
            if (returnCode == ErrorCode.GameIdAlreadyExists)
            {
                JoinRoomWithCode(TEST_ROOM_CODE);
            }
        }

        /// <summary>
        /// Join room with code function
        /// </summary>
        /// <param name="code">The lobby code</param>
        private void JoinRoomWithCode(string code)
        {
            PhotonNetwork.JoinRoom(code);
        }

        /// <summary>
        /// Handle create room event
        /// </summary>
        /// <param name="data"></param>
        private void HandleCreateRoom(BytesData data)
        {
            CreateRoomWithRandomCode();
        }

        private void CreateRoomWithRandomCode()
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            Debug.Log($"masterclient is: {PhotonNetwork.LocalPlayer}");
            PhotonNetwork.CreateRoom(TEST_ROOM_CODE, BuildRoomOptions());
        }

        private RoomOptions BuildRoomOptions()
        {
            return new RoomOptions()
            {
                IsVisible = true,
                IsOpen = true,
                PublishUserId = true,
                MaxPlayers = (byte)MAX_LOBBY_SIZE
            };
        }

        /// <summary>
        /// Handle join room event
        /// </summary>
        /// <param name="data"></param>
        private void HandleJoinRoom(BytesData data)
        {
            //var roomData = data as StringDataBytes;
            string roomCode = TEST_ROOM_CODE;//roomData.StringValue;

            if (roomCode != String.Empty)
            {
                JoinRoomWithCode(roomCode);
            }
        }

        static private string CreateRandomRoomCode()
        {
            string alphabet = "abcdejfhijklmnopqrstuvwxyz".ToUpper();
            string digit = "0123456789";

            return $"{GetRandomCharacterFrom(alphabet)}{GetRandomCharacterFrom(digit)}{GetRandomCharacterFrom(alphabet)}{GetRandomCharacterFrom(digit)}";
        }

        static private char GetRandomCharacterFrom(string aString)
        {
            return aString[UnityEngine.Random.Range(0, aString.Length)];
        }
    }
}