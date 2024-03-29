using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Kraken.Network;
using Newtonsoft.Json;

namespace Kraken
{
    public class Tower : MonoBehaviourPun
    {
        public static string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "towers.json");
        }

        public enum TowerState
        {
            Inactive = 0,
            Waiting = 1,
            Active = 2
        }

        public int Id { get; set; }
        [SerializeField] private GameObject _waitingPrefab;
        [SerializeField] private GameObject _towerPrefab;

        private List<string> _playersInRange = new List<string>();
        private TowerState _towerState;
        private LilWoolManager _lilWoolManager = null;

        private void OnTriggerEnter(Collider other)
        {
            var towerInteract = other.GetComponent<PlayerTowerInteractComponent>();

            if (towerInteract)
            {
                towerInteract.SetNewTowerInRange(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var towerInteract = other.GetComponent<PlayerTowerInteractComponent>();

            if (towerInteract)
            {
                towerInteract.SetNewTowerInRange(null);
            }
        }

        public void PlayerTryBuild()
        {
            string id = PhotonNetwork.LocalPlayer.UserId;
            photonView.RPC(nameof(RPC_Master_AddPlayerInRange), RpcTarget.MasterClient, id);
        }

        [PunRPC]
        private void RPC_Master_AddPlayerInRange(string id)
        {
            if (!_playersInRange.Contains(id))
                _playersInRange.Add(id);

            if (_playersInRange.Count == PhotonNetwork.PlayerList.Length)
            {
                if (_towerState == TowerState.Inactive)
                {
                    SetNewTowerState(TowerState.Waiting);
                }
            }
        }

        [PunRPC]
        private void RPC_Master_RemovePlayerInRange(string id)
        {
            _playersInRange.Remove(id);
        }

        public void PlayerCancelBuild()
        {
            string id = PhotonNetwork.LocalPlayer.UserId;
            photonView.RPC(nameof(RPC_Master_RemovePlayerInRange), RpcTarget.MasterClient, id);
        }

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;
        }

        public void SetNewTowerState(TowerState newState)
        {
            _towerState = newState;

            if(_towerState == TowerState.Waiting)
            {
                NetworkUtils.Instantiate(_waitingPrefab.name, transform.position, transform.rotation);
                TowerManager.Instance.TowerBuilt();
            }
            else if(_towerState == TowerState.Active)
            {
                NetworkUtils.Instantiate(_towerPrefab.name, transform.position, transform.rotation);
            }

            if(!TowerManager.Instance.TowerData.TryAdd(Id, (int)_towerState))
            {
                TowerManager.Instance.TowerData[Id] = (int)_towerState;
            }
            TowerManager.Instance.WriteFile();
        }
    }
}