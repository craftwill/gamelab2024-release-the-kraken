using Bytes;
using Kraken.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class TowerUI : MonoBehaviourPun
    {
        //only one can be visible at a time
        private GameObject _visibleGameObject = null;
        private int _playerId = 1;
        private bool _playerIsInZone = false;
        private bool _otherPlayerWaiting = false;
        private bool _youWaiting = false;
        private int _woolQuantity = 0;
        private LilWoolManager _woolManager;
        private bool _isC = true;
        private bool canShowInput = false;

        private GameObject _calling;
        private GameObject _waiting;

        [SerializeField] private GameObject _callDazzle;
        [SerializeField] private GameObject _callRazzle;
        [SerializeField] private GameObject _dazzleWaiting;
        [SerializeField] private GameObject _razzleWaiting;
        [SerializeField] private GameObject _towerAvailable;
        [SerializeField] private GameObject _mkInput;
        [SerializeField] private GameObject _cInput;

        private void Start()
        {
            //0 = razzle, 2 = dazzle
            _playerId = MultiplayerGameManager.GetPlayerClassId();
            _calling = _playerId == 0 ? _callDazzle : _callRazzle;
            _waiting = _playerId == 0 ? _dazzleWaiting : _razzleWaiting;

            _woolManager = FindAnyObjectByType<LilWoolManager>(FindObjectsInactive.Exclude);

            EventManager.AddEventListener(EventNames.TowerAttemptBuilt, HandleTowerAttemptBuilt);
            EventManager.AddEventListener(EventNames.TowerCancelBuilt, HandleTowerCancelBuilt);
            EventManager.AddEventListener(EventNames.TowerBuilt, HandleTowerBuilt);
            EventManager.AddEventListener(EventNames.PlayerEnteredTower, HandlePlayerEnteredZone);
            EventManager.AddEventListener(EventNames.PlayerLeftTower, HandlePlayerLeftZone);
            EventManager.AddEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
            EventManager.AddEventListener(EventNames.InputSchemeChanged, HandleInputSchemeChanged);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.TowerAttemptBuilt, HandleTowerAttemptBuilt);
            EventManager.RemoveEventListener(EventNames.TowerCancelBuilt, HandleTowerCancelBuilt);
            EventManager.RemoveEventListener(EventNames.TowerBuilt, HandleTowerBuilt);
            EventManager.RemoveEventListener(EventNames.PlayerEnteredTower, HandlePlayerEnteredZone);
            EventManager.RemoveEventListener(EventNames.PlayerLeftTower, HandlePlayerLeftZone);
            EventManager.RemoveEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
            EventManager.RemoveEventListener(EventNames.InputSchemeChanged, HandleInputSchemeChanged);
        }

        private void HandleTowerAttemptBuilt(BytesData bytes)
        {
            _youWaiting = true;
            photonView.RPC(nameof(OtherPlayerIsWaitingOnTower), RpcTarget.Others);
            UpdateTowerUI();
        }

        private void HandleTowerCancelBuilt(BytesData bytes)
        {
            _youWaiting = false;
            photonView.RPC(nameof(OtherPlayerCancelTower), RpcTarget.Others);
            UpdateTowerUI();
        }

        private void HandleTowerBuilt(BytesData bytes)
        {
            _playerIsInZone = false;
            _youWaiting = false;
            _otherPlayerWaiting = false;
            UpdateTowerUI();
        }

        private void HandlePlayerEnteredZone(BytesData bytes)
        {
            Zone z = (bytes as ZoneEventData).Zone;
            if(z.GetTowerState() == Tower.TowerState.Inactive)
            {
                _playerIsInZone = true;
                UpdateTowerUI();
                
            }
        }

        private void HandlePlayerLeftZone(BytesData bytes)
        {
            Zone z = (bytes as ZoneEventData).Zone;
            if (z.GetTowerState() == Tower.TowerState.Inactive)
            {
                _playerIsInZone = false;
                UpdateTowerUI();
            }
        }

        private void HandleUpdateWoolQuantity(BytesData bytes)
        {
            _woolQuantity = ((IntDataBytes)bytes).IntValue;
            UpdateTowerUI();
        }

        private void HandleInputSchemeChanged(BytesData bytes)
        {
            _isC = (bytes as StringDataBytes).StringValue.Equals("Gamepad");
            SetNewVisibleUI(_visibleGameObject);
        }

        private void UpdateTowerUI()
        {
            if (_otherPlayerWaiting)
            {
                SetNewVisibleUI(_waiting);
            }
            else if (_youWaiting)
            {
                SetNewVisibleUI(_calling);
            }
            else if(_playerIsInZone && _woolQuantity >= Config.current.towerWoolCost && TowerManager.Instance.TowersBuiltThisRound < Config.current.maxTowerPerRound)
            {
                SetNewVisibleUI(_towerAvailable);
            }
            else
            {
                SetNewVisibleUI(null);
            }
        }

        private void SetNewVisibleUI(GameObject g)
        {
            
            if (g != _visibleGameObject)
            {
                _visibleGameObject?.SetActive(false);
                _visibleGameObject = g;
                _visibleGameObject?.SetActive(true);
            }
            
            if (_visibleGameObject && _playerIsInZone)
            {
                _cInput.SetActive(_isC);
                _mkInput.SetActive(!_isC);
            }
            else
            {
                _cInput.SetActive(false);
                _mkInput.SetActive(false);
            }
            
        }

        [PunRPC]
        private void OtherPlayerIsWaitingOnTower()
        {
            _otherPlayerWaiting = true;
            UpdateTowerUI();
        }

        [PunRPC]
        private void OtherPlayerCancelTower()
        {
            _otherPlayerWaiting = false;
            UpdateTowerUI();
        }
    }
}