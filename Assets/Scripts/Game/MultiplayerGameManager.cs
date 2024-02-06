using Bytes;
using Kraken.Network;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Kraken
{
    [DisallowMultipleComponent]
    public class MultiplayerGameManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        int playerCount = 0;

        [SerializeField] private Transform _playersRespawnPos;
        [SerializeField] private Transform _playersSpawnPos;
        [SerializeField] private Transform _respawnBoxTeleportPos;
        [SerializeField] private GameObject _playerPrefab;

        private GameObject _localPlayer;
        private List<PlayerEntity> playerEntities;

        private bool _gameStarted;

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
        }

        private void Update()
        {
            
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            // Remove Player Entity that matches the UserId
            playerEntities.RemoveAll((playerEntity) =>
            {
                return playerEntity.photonView.Owner.UserId == otherPlayer.UserId;
            });
        }

        private void CreatePlayer()
        {
            Debug.Log("Creating player!");

            _localPlayer = NetworkUtils.Instantiate(_playerPrefab.name, _playersSpawnPos.position);
        }

        private void TeleportPlayer(string playerUserId, Vector3 position)
        {
            var playerToTeleport = GetPlayerEntity(playerUserId);

            // Sanity check
            if (playerToTeleport == null) { return; }

            TeleportPlayer(playerToTeleport.photonView, position);
        }

        private void TeleportPlayer(PhotonView playerToTeleport, Vector3 position) 
        {
            playerToTeleport.transform.position = position;
        }

        private PlayerEntity GetPlayerEntity(string playerUserId) 
        {
            return playerEntities.Find(playerEntity => playerEntity.photonView.Owner.UserId == playerUserId);
        }
    }
}