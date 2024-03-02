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
        private int _enemyCount = 0;
        private bool _isCurrentlyFull = false;
        private bool _isActiveZone = false;
        private List<EnemyEntity> _enemyInZones;
        private Animate _animOccupancyTimer;

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) return;

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

            if (_isActiveZone)
            {
                EventManager.Dispatch(EventNames.UpdateCurrentZoneOccupancyUI, new UpdateZoneOccupancyUIData(_enemyCount, _maxEnemyCount));
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
        }
    }
}
