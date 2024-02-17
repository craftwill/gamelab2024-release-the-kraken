using Kraken.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

using Bytes;

namespace Kraken
{
    [DisallowMultipleComponent]
    public class MultiplayerGameManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        [SerializeField] private Transform _playersRespawnPos;
        [SerializeField] private Transform _playersSpawnPos;
        [SerializeField] private Transform _respawnBoxTeleportPos;
        [SerializeField] private GameObject _playerPrefab;

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

                // Stop gameflow for testing purposes after 12 seconds.
                Animate.Delay(12f, () =>
                {
                    TryStopGameFlow();
                });
            }
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

            _localPlayer = NetworkUtils.Instantiate(_playerPrefab.name, _playersSpawnPos.position);

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