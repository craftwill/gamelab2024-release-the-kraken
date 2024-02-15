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

            Animate.Delay(1f, () => 
            {
                EventManager.Dispatch(EventNames.StartGameFlow, null);
                
                // Stop gameflow for testing purposes after 8 seconds.
                Animate.Delay(8f, () =>
                {
                    EventManager.Dispatch(EventNames.StopGameFlow, null);
                });
            });
        }

        private void Update()
        {
            
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
        }

        private void CreatePlayer()
        {
            Debug.Log("Creating player!");

            _localPlayer = NetworkUtils.Instantiate(_playerPrefab.name, _playersSpawnPos.position);
        }

        private void TeleportPlayer(PhotonView playerToTeleport, Vector3 position) 
        {
            playerToTeleport.transform.position = position;
        }
    }
}