using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using Bytes;
using Kraken.Network;

namespace Kraken.Game
{
    public class SpawnOnStartDebug : MonoBehaviourPun
    {
        public string entityToSpawnName;
        public float spawnStartDelay;

        private Transform[] _spawnPoints;

        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            _spawnPoints = gameObject.GetComponentsInChildren<Transform>();
        }

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Animate.Delay(spawnStartDelay, SpawnEntitiesAtEverySpawnPoint, true);
        }

        private void SpawnEntitiesAtEverySpawnPoint()
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                SpawnEntity(spawnPoint.position);
            }
        }

        private void SpawnEntity(Vector3 spawnPos) 
        {
            NetworkUtils.Instantiate(entityToSpawnName, spawnPos);
        }
    }
}
