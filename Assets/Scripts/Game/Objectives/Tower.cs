using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Kraken.Network;
using Newtonsoft.Json;
using Bytes;

namespace Kraken
{
    public class Tower : MonoBehaviourPun
    {
        public enum TowerState
        {
            Inactive = 0,
            Waiting = 1,
            Active = 2
        }

        [SerializeField] private int _id;
        [SerializeField] private GameObject _waitingPrefab;
        [SerializeField] private GameObject _towerPrefab;
        [SerializeField] private GameObject _inactivePrefab;

        private GameObject _spawnedInactive = null;

        private ZoneEventData _zoneEventData;
        private List<string> _playersInRange = new List<string>();
        public TowerState _TowerState { get; private set; }
        public int _playersCount = 0;

        private void Start()
        {
            _zoneEventData = new ZoneEventData(transform.parent.GetComponent<Zone>());
        }

        private void OnTriggerEnter(Collider other)
        {
            var towerInteract = other.GetComponent<PlayerTowerInteractComponent>();

            if (towerInteract)
            {
                if (other.GetComponent<PlayerEntity>()._isOwner)
                {
                    EventManager.Dispatch(EventNames.PlayerEnteredTower, _zoneEventData);
                }
                
                towerInteract.SetNewTowerInRange(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var towerInteract = other.GetComponent<PlayerTowerInteractComponent>();

            if (towerInteract)
            {
                if (other.GetComponent<PlayerEntity>()._isOwner)
                {
                    EventManager.Dispatch(EventNames.PlayerLeftTower, _zoneEventData);
                }

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
            photonView.RPC(nameof(RPC_All_UpdatePlayersInRangeCount), RpcTarget.All, _playersCount + 1);


            if (_playersInRange.Count == PhotonNetwork.PlayerList.Length)
            {
                if (_TowerState == TowerState.Inactive)
                {
                    SetNewTowerState(TowerState.Waiting);
                }
            }
        }

        [PunRPC]
        private void RPC_All_UpdatePlayersInRangeCount(int count)
        {
            _playersCount = count;
        }

        [PunRPC]
        private void RPC_Master_RemovePlayerInRange(string id)
        {
            _playersInRange.Remove(id);
            photonView.RPC(nameof(RPC_All_UpdatePlayersInRangeCount), RpcTarget.All, _playersCount - 1);
        }

        public void PlayerCancelBuild()
        {
            string id = PhotonNetwork.LocalPlayer.UserId;
            photonView.RPC(nameof(RPC_Master_RemovePlayerInRange), RpcTarget.MasterClient, id);
        }

        public void SetNewTowerState(TowerState newState)
        {
            _TowerState = newState;

            if(_TowerState == TowerState.Waiting)
            {
                PhotonNetwork.Destroy(_spawnedInactive);
                NetworkUtils.Instantiate(_waitingPrefab.name, transform.position, transform.rotation);
                TowerManager.Instance.TowerBuilt();
            }
            else if(_TowerState == TowerState.Active)
            {
                NetworkUtils.Instantiate(_towerPrefab.name, transform.position, transform.rotation);
            }
            else if(_TowerState == TowerState.Inactive)
            {
                _spawnedInactive = NetworkUtils.Instantiate(_inactivePrefab.name, transform.position, transform.rotation);
            }

            if(!TowerManager.Instance.TowerData.TryAdd(_id, (int)_TowerState))
            {
                TowerManager.Instance.TowerData[_id] = (int)_TowerState;
            }
            TowerManager.Instance.WriteFile();
        }

        public TowerState GetTowerState()
        {
            return _TowerState;
        }

        public int GetId()
        {
            return _id;
        }
    }
}