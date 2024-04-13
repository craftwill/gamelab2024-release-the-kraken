using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerTowerInteractComponent : MonoBehaviourPun
    {
        [SerializeField] private PlayerSoundComponent _soundComponent;
        private Tower _towerInRange = null;
        private LilWoolManager _lilWoolManager = null;
        
        private void Start()
        {
            _lilWoolManager = FindAnyObjectByType<LilWoolManager>(FindObjectsInactive.Exclude);
        }

        public void OnTowerInteractPressed()
        {
            if (_towerInRange &&_lilWoolManager?._woolQuantity >= Config.current.towerWoolCost && TowerManager.Instance.TowersBuiltThisRound < Config.current.maxTowerPerRound)
            {
                if(_towerInRange._TowerState == Tower.TowerState.Inactive)
                {
                    if (_towerInRange.getPlayerCount() == 0)
                        photonView.RPC(nameof(_soundComponent.RPC_Other_PlayTowerWaitingSound), RpcTarget.Others);
                    EventManager.Dispatch(EventNames.TowerAttemptBuilt, null);
                    _towerInRange.PlayerTryBuild();
                }
            }
        }

        public void OnTowerInteractCanceled()
        {
            if (_towerInRange)
            {
                if (_towerInRange._TowerState == Tower.TowerState.Inactive)
                {
                    EventManager.Dispatch(EventNames.TowerCancelBuilt, null);
                    _towerInRange.PlayerCancelBuild();
                }
            }
        }

        public void SetNewTowerInRange(Tower t)
        {
            if (t is null)
            {
                EventManager.Dispatch(EventNames.TowerCancelBuilt, null);
                _towerInRange.PlayerCancelBuild();
            }
            _towerInRange = t;
        }
    }
}