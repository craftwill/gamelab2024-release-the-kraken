using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

using Bytes;

using Kraken;

namespace Kraken.Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        const string TEST_ROOM_CODE = "test";

        public void Init()
        {
            TryConnect();
            PhotonNetwork.AutomaticallySyncScene = true;

            EventManager.AddEventListener(EventNames.CreateRoom, HandleCreateRoom);
            EventManager.AddEventListener(EventNames.JoinRoomWithCode, HandleJoinRoom);
            EventManager.AddEventListener(EventNames.JoinGameScene, HandleJoinGameScene);
            EventManager.AddEventListener(EventNames.SetLocalPlayerNickName, HandleSetLocalPlayerNickName);
        }

        private void TryConnect()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Logged in the master server in " + PhotonNetwork.CloudRegion + " region!");
            PhotonNetwork.AutomaticallySyncScene = true;

            // Automaticlly create a room a join it
            if (Config.current.isSkipMainMenu)
            {
                CreateRoomWithCode(TEST_ROOM_CODE);
            }

            // Create NickName or retrieve it from PlayerPrefs if it already exists
            if (!GameManager.IsLocalPlayerNickNameSet())
            {
                // Pop-up to ask Player to set a username
                //EventManager.Dispatch(EventNames.PopupSetLocalPlayerNickName, null);
            }
            else
            {
                // Needs to be assigned every time you connect to the Master Server
                PhotonNetwork.NickName = GameManager.GetLocalPlayerNickName();
                Debug.Log($"Your nickname is: {PhotonNetwork.NickName}");
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            Debug.Log("Disconnected!" + cause.ToString());
        }

        /// <summary>
        /// On player joined room callback function
        /// </summary>
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            Debug.Log("On Joined Room!");
            if (!PhotonNetwork.IsMasterClient) return;

            // Directly load the multiplayerGameScene to skip the lobby
            if (Config.current.isSkipMainMenu)
            {
                LoadMultiplayerGameScene();
                return;
            }

            // Load lobby view
            PhotonNetwork.LoadLevel(1);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            // If played failed to create the TEST_ROOM_CODE room, then it must already exist. Join it instead. 
            if (returnCode == ErrorCode.GameIdAlreadyExists && Config.current.isSkipMainMenu)
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
            PhotonNetwork.CreateRoom(NetworkUtils.CreateRandomRoomCode(), BuildRoomOptions());
        }

        private void CreateRoomWithCode(string code)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            Debug.Log($"masterclient is: {PhotonNetwork.LocalPlayer}");
            PhotonNetwork.CreateRoom(code, BuildRoomOptions());
        }

        private RoomOptions BuildRoomOptions() 
        {
            return new RoomOptions()
            {
                IsVisible = true,
                IsOpen = true,
                PublishUserId = true,
                MaxPlayers = (byte)NetworkUtils.MAX_LOBBY_SIZE
            };
        }

        /// <summary>
        /// Handle join room event
        /// </summary>
        /// <param name="data"></param>
        private void HandleJoinRoom(BytesData data)
        {
            var roomData = data as StringDataBytes;
            string roomCode = roomData.StringValue;

            if (roomCode != String.Empty)
            {
                JoinRoomWithCode(roomCode);
            }
        }

        /// <summary>
        /// Handle join game scene
        /// </summary>
        /// <param name="data"></param>
        private void HandleJoinGameScene(BytesData data)
        {
            LoadMultiplayerGameScene();
        }

        private void LoadMultiplayerGameScene() 
        {
            var customSceneName = Config.current.loadCustomSceneName;
            if (customSceneName != "")
            {
                Debug.LogWarning("You loaded a custom scene with name: " + customSceneName);
                PhotonNetwork.LoadLevel(customSceneName);
                return;
            }
            PhotonNetwork.LoadLevel(2);
        }

        /// <summary>
        /// Once a player has set a NickName in the pop up, this is called.
        /// It will update the Networked name and saves it locally in the PlayerPrefs.
        /// </summary>
        private void HandleSetLocalPlayerNickName(BytesData data)
        {
            var nameData = (data as StringDataBytes).StringValue;
            PhotonNetwork.LocalPlayer.NickName = nameData;
            GameManager.SetLocalPlayerNickName(nameData);
            Debug.Log($"NetworkManager -- Your nickname is now: {PhotonNetwork.LocalPlayer.NickName}");
        }
    }
}