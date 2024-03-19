using Kraken.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

using Bytes;
using UnityEngine.SceneManagement;

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

        private void HandleLeaveGame(BytesData data) 
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
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("player"))
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("player", out var value))
                {
                    int classId = (int)value;
                    if (classId == 0)
                    {
                        playerToCreateName = _razzlePrefab.name;
                        playerToCreateSpawnPos = Config.current.razzleSpawnPoint;
                    }
                    else if (classId == 2)
                    {
                        playerToCreateName = _dazzlePrefab.name;
                        playerToCreateSpawnPos = Config.current.dazzleSpawnPoint;
                    }
                }
            }
            else
            {
                // Player2 is dazzle by default if custom properties are null
                if (!PhotonNetwork.IsMasterClient)
                {
                    playerToCreateName = _dazzlePrefab.name;
                    playerToCreateSpawnPos = Config.current.dazzleSpawnPoint;
                }
            }

            // Force usage of Dazzle for player1
            if (Config.current.forceUseDazzlePlayer1) 
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    playerToCreateName = _dazzlePrefab.name;
                    playerToCreateSpawnPos = Config.current.razzleSpawnPoint;
                }   
                else
                {
                    playerToCreateName = _razzlePrefab.name;
                    playerToCreateSpawnPos = Config.current.dazzleSpawnPoint;
                }
            }

            _localPlayer = NetworkUtils.Instantiate(playerToCreateName, playerToCreateSpawnPos);

            photonView.RPC(nameof(RPC_Master_PlayerCreated), RpcTarget.MasterClient, _localPlayer.GetPhotonView().ViewID);
        }

        [PunRPC]
        private void RPC_Master_PlayerCreated(int playerCreatedViewId)
        {
            _playerCount++;
            Debug.Log("Player created! Player count: " + _playerCount);

            if (_playerCount >= 2)
            {
                TryStartGameFlow();
            }
        }
    }
}