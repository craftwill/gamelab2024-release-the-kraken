
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using Bytes;

using Kraken.Network;

namespace Kraken
{
    [DisallowMultipleComponent]
    public class MultiplayerGameManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        [SerializeField] private Transform _playersRespawnPos;
        [SerializeField] private Transform _playersSpawnPos;
        [SerializeField] private Transform _respawnBoxTeleportPos;
        [SerializeField] private GameObject _razzlePrefab;
        [SerializeField] private GameObject _dazzlePrefab;

        private PlayerInput _playerInput;
        private GameObject _localPlayer;
        private bool _gameStarted = false;
        private bool _gameEnded = false;
        private int _playerCount = 0;

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Start()
        {
            EventManager.AddEventListener(EventNames.LeaveGame, HandleLeaveGame);

            CreatePlayer();
            _playerInput = _localPlayer.GetComponent<PlayerInput>();

            // Setup HUD according to player type and control schema
            int playerClassId = GetPlayerClassId();
            bool isRazzle = playerClassId == 0;
            bool isKeyboard = _playerInput.currentControlScheme.Equals("Gamepad");

            // Delay to init HUD since events listeners are added in Start()
            Animate.Delay(0.02f, () => 
            {
                EventManager.Dispatch(EventNames.SetupHUD, new SetupHUDData(isRazzle, isKeyboard));
            });

            if (!PhotonNetwork.IsMasterClient) return;

            // Skip need to wait for two players to have joined.
            if (Config.current.useDebugGameFlow)
            {
                // Start gameflow for testing purposes after 1 seconds.
                Animate.Delay(1f, () =>
                {
                    TryStartGameFlow();
                });
            }
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.LeaveGame, HandleLeaveGame);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            photonView.RPC(nameof(RPC_ALL_LeaveRoom), RpcTarget.All);

        }

        private void HandleLeaveGame(BytesData data) 
        {
            photonView.RPC(nameof(RPC_ALL_LeaveRoom), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_ALL_LeaveRoom()
        {
            AnimateManager.GetInstance().ClearAllAnimations();
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            SceneManager.LoadScene(1); // Back to main menu
        }

        public void TryStartGameFlow()
        {
            if (_gameStarted) return;

            if (_gameEnded)
            {
                Debug.LogError("Can't start the game again! It's ended and need to go back to lobby first!");
                return;
            }    

            _gameStarted = true;
            Time.timeScale = 1f;
            EventManager.Dispatch(EventNames.StartGameFlow, null);
        }

        public void TryStopGameFlow()
        {
            if (_gameEnded) return;

            _gameEnded = true;
            _gameStarted = false;
            EventManager.Dispatch(EventNames.StopGameFlow, null);
        }

        private void CreatePlayer()
        {
            Debug.Log("Creating player!");

            // Fetch property for player type
            string playerToCreateName = _razzlePrefab.name;
            Vector3 playerToCreateSpawnPos = _playersSpawnPos.position;
            Vector3 playerToCreateSpawnRotation = _playersSpawnPos.rotation.eulerAngles;

            int playerClassId = GetPlayerClassId();
            if (playerClassId == 0)
            {
                playerToCreateName = _razzlePrefab.name;
                playerToCreateSpawnPos = Config.current.razzleSpawnPoint;
                playerToCreateSpawnRotation = Config.current.razzleSpawnRotation;
            }
            else if (playerClassId == 2)
            {
                playerToCreateName = _dazzlePrefab.name;
                playerToCreateSpawnPos = Config.current.dazzleSpawnPoint;
                playerToCreateSpawnRotation = Config.current.dazzleSpawnRotation;
            }

            // Force usage of Dazzle for player1
            if (Config.current.forceUseDazzlePlayer1) 
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    playerToCreateName = _dazzlePrefab.name;
                    playerToCreateSpawnPos = Config.current.razzleSpawnPoint;
                    playerToCreateSpawnRotation = Config.current.razzleSpawnRotation;
                }
                else
                {
                    playerToCreateName = _razzlePrefab.name;
                    playerToCreateSpawnPos = Config.current.dazzleSpawnPoint;
                    playerToCreateSpawnRotation = Config.current.dazzleSpawnRotation;
                }
            }

            _localPlayer = NetworkUtils.Instantiate(playerToCreateName, playerToCreateSpawnPos, Quaternion.Euler(playerToCreateSpawnRotation));

            photonView.RPC(nameof(RPC_Master_PlayerCreated), RpcTarget.MasterClient, _localPlayer.GetPhotonView().ViewID);
        }

        public static int GetPlayerClassId() 
        {
            // Player2 is dazzle by default if custom properties are null
            int playerClassId = 2;
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("player"))
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("player", out var value))
                {
                    playerClassId = (int)value;
                }
            }
            return playerClassId;
        }

        [PunRPC]
        private void RPC_Master_PlayerCreated(int playerCreatedViewId)
        {
            _playerCount++;

            if (_playerCount >= 2)
            {
                TryStartGameFlow();
            }
        }
    }
}