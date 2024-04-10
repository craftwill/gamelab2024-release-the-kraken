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
        private int _woolQuantity = 0;
        private LilWoolManager _woolManager;

        [SerializeField] private GameObject _callDazzle;
        [SerializeField] private GameObject _callRazzle;
        [SerializeField] private GameObject _dazzleWaiting;
        [SerializeField] private GameObject _razzleWaiting;
        [SerializeField] private GameObject _towerAvailable;

        // Start is called before the first frame update
        void Start()
        {
            //0 = razzle, 2 = dazzle
            _playerId = MultiplayerGameManager.GetPlayerClassId();

            _woolManager = FindAnyObjectByType<LilWoolManager>(FindObjectsInactive.Exclude);

            EventManager.AddEventListener(EventNames.TowerAttemptBuilt, HandleTowerAttemptBuilt);
            EventManager.AddEventListener(EventNames.TowerCancelBuilt, HandleTowerCancelBuilt);
            EventManager.AddEventListener(EventNames.PlayerEnteredObjective, HandlePlayerEnteredZone);
            EventManager.AddEventListener(EventNames.PlayerLeftObjective, HandlePlayerLeftZone);
            EventManager.AddEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
        }

        private void HandleTowerAttemptBuilt(BytesData bytes)
        {
            if(_visibleGameObject == _towerAvailable)
            {
                GameObject g = _playerId == 0 ? _callRazzle : _callDazzle;
                SetNewVisibleUI(g);
                photonView.RPC(nameof(OtherPlayerIsCalling), RpcTarget.Others);
            }
        }

        private void HandleTowerCancelBuilt(BytesData bytes)
        {

        }

        private void HandlePlayerEnteredZone(BytesData bytes)
        {
            _playerIsInZone = true;
            UpdateTowerAvailable();
        }

        private void HandleUpdateWoolQuantity(BytesData bytes)
        {
            _woolQuantity = ((IntDataBytes)bytes).IntValue;
            UpdateTowerAvailable();
        }

        private void HandlePlayerLeftZone(BytesData bytes)
        {
            _playerIsInZone = false;
            UpdateTowerAvailable();
        }

        private void UpdateTowerAvailable()
        {
            if(_visibleGameObject is null && _playerIsInZone && _woolQuantity >= Config.current.towerWoolCost)
            {
                SetNewVisibleUI(_towerAvailable);
            }
        }

        private void SetNewVisibleUI(GameObject g)
        {
            _visibleGameObject?.SetActive(false);
            _visibleGameObject = g;
            _visibleGameObject.SetActive(true);
        }

        [PunRPC]
        private void OtherPlayerIsCalling()
        {
            GameObject g = _playerId == 0 ? _dazzleWaiting : _razzleWaiting;
            SetNewVisibleUI(g);
        }
    }
}