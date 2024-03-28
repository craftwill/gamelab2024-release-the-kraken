using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class Tower : MonoBehaviourPun
    {
        private enum TowerState
        {
            Inactive,
            Waiting,
            Active
        }

        private List<PlayerTowerInteractComponent> _playersInRange = new List<PlayerTowerInteractComponent>();
        private TowerState _towerState;

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

        public void PlayerTryBuild(PlayerTowerInteractComponent player)
        {
            //circular reference baby
            if (!_playersInRange.Contains(player))
                _playersInRange.Add(player);

            if (_playersInRange.Count == 2)
            {
                if(_towerState == TowerState.Inactive)
                {
                    _towerState = TowerState.Waiting;
                }
            }
        }

        public void PlayerCancelBuild(PlayerTowerInteractComponent player)
        {
            _playersInRange.Remove(player);
        }

        private void Start()
        {
            //var test = PhotonNetwork.LocalPlayer.CustomProperties;
            //Debug.Log(test.Keys.Count);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}