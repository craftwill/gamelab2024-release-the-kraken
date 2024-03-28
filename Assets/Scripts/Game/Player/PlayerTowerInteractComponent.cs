using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerTowerInteractComponent : MonoBehaviourPun
    {
        private Tower _towerInRange = null;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnTowerInteractPressed()
        {
            if (_towerInRange)
            {
                _towerInRange.PlayerTryBuild(this);
            }
        }

        public void OnTowerInteractCanceled()
        {
            if (_towerInRange)
            {
                _towerInRange.PlayerCancelBuild(this);
            }
        }

        public void SetNewTowerInRange(Tower t)
        {
            if (t is null)
            {
                _towerInRange.PlayerCancelBuild(this);
            }
            _towerInRange = t;
        }
    }
}