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
        // Start is called before the first frame update
        void Start()
        {
            _lilWoolManager = FindAnyObjectByType<LilWoolManager>(FindObjectsInactive.Exclude);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnTowerInteractPressed()
        {
            if (_towerInRange)
            {
                if(_lilWoolManager?._woolQuantity >= 0)
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