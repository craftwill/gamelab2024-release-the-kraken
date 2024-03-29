using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerTowerInteractComponent : MonoBehaviourPun
    {
        private Tower _towerInRange = null;
        private LilWoolManager _lilWoolManager = null;
        private TowerManager _towerManager = null;
        
        private void Start()
        {
            _lilWoolManager = FindAnyObjectByType<LilWoolManager>(FindObjectsInactive.Exclude);
            _towerManager = FindAnyObjectByType<TowerManager>(FindObjectsInactive.Exclude);
        }

        public void OnTowerInteractPressed()
        {
            if (_towerInRange &&_lilWoolManager?._woolQuantity >= 0 && TowerManager.Instance.TowersBuiltThisRound < Config.current.maxTowerPerRound)
            {
                Debug.Log("here");
                _towerInRange.PlayerTryBuild();
            }
        }

        public void OnTowerInteractCanceled()
        {
            if (_towerInRange)
            {
                //_towerInRange.PlayerCancelBuild(this);
            }
        }

        public void SetNewTowerInRange(Tower t)
        {
            if (t is null)
            {
                //_towerInRange.PlayerCancelBuild();
            }
            _towerInRange = t;
        }
    }
}