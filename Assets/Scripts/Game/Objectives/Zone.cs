using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class Zone : MonoBehaviourPun
    {
        [SerializeField, Tooltip("Maximum zone occupancy")] private int _maxEnemyCount = 100;
        [SerializeField, Tooltip("Time before loss after maximum zone occupancy is reached")] private int _maxZoneOccupancyTimer = 30;
        [SerializeField, Tooltip("The spawner object in the scene with its spawn points as children")] private Spawner _spawner;
        [SerializeField, Tooltip("The visual indicator for the zone occupation in the minimap")] private MinimapZoneOccupationUI _minimapIndicator;
        [SerializeField] private Tower _tower;
        private int _enemyCount = 0;
        private int _playerCount = 0;
        private bool _isCurrentlyFull = false;
        private bool _isActiveZone = false;
        private List<EnemyEntity> _enemyInZones;
        private Animate _animOccupancyTimer;

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (_isActiveZone)
            {
                // Objective music
                var player = other.GetComponent<PlayerEntity>();
                if (player is not null)
                {
                    _playerCount++;
                    EventManager.Dispatch(EventNames.PlayerEnteredObjective, null);
                }
            }

            //This trigger is on a gameobject with ZoneOccupancy Layer
            var ezc = other.GetComponent<EnemyZoneComponent>();
            int zoneCount = 0;
            if(ezc is not null)
            {
                ezc.SetZoneToEnemy(this);
                zoneCount = ezc.ZoneCount;
            }

            ChangeEnemyCount(zoneCount);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            if (_isActiveZone)
            {
                // Objective music
                var player = other.GetComponent<PlayerEntity>();
                if (player is not null)
                {
                    _playerCount--;
                    EventManager.Dispatch(EventNames.PlayerLeftObjective, null);
                }
            }

            //This trigger is on a gameobject with ZoneOccupancy Layer
            var ezc = other.GetComponent<EnemyZoneComponent>();
            int zoneCount = 0;
            if(ezc is not null)
            {
                ezc.RemoveZoneToEnemy(this);
                zoneCount = ezc.ZoneCount;
            }

            ChangeEnemyCount(-zoneCount);
        }

        public void ChangeEnemyCount(int zoneCount)
        {
            _enemyCount += zoneCount;
            _minimapIndicator?.SetOccupation(_enemyCount, _maxEnemyCount);

            if (_isActiveZone)
            {
                //commenting it in case we want to reuse stuff
                //EventManager.Dispatch(EventNames.UpdateCurrentZoneOccupancyUI, new UpdateZoneOccupancyUIData(_enemyCount, _maxEnemyCount));
            }

            if (_enemyCount >= _maxEnemyCount)
            {
                _isCurrentlyFull = true;
                if (_animOccupancyTimer == null)
                {
                    _animOccupancyTimer = Animate.Delay(_maxZoneOccupancyTimer, () =>
                    {
                        if (_isCurrentlyFull)
                        {
                            EventManager.Dispatch(EventNames.ZoneFullLoss, null);
                        }
                    }, true);
                }
            }
            else
            {
                _isCurrentlyFull = false;
                // Cancel occupancy timer
                if (_animOccupancyTimer != null)
                {
                    _animOccupancyTimer?.Stop(callEndFunction: false);
                    _animOccupancyTimer = null;
                }
            }
        }

        public Spawner GetSpawner()
        {
            return _spawner;
        }

        public void SetIsActiveZone(bool isActiveZone) 
        {
            _isActiveZone = isActiveZone;
            if (_playerCount > 0)
            {
                if (isActiveZone)
                    EventManager.Dispatch(EventNames.PlayerEnteredObjective, null);
                else
                    EventManager.Dispatch(EventNames.PlayerLeftObjective, null);
            }
        }

        public bool ZoneHasTower()
        {
            return _tower.GetTowerState() == Tower.TowerState.Active;
        }
    }
}
