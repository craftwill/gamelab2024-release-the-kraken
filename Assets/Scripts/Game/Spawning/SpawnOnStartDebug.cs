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
        [SerializeField] private EnemySpawnPack[] _enemySpawnPacks;
        [SerializeField] private float _spawnStartDelay;
        [SerializeField] private float _spawnWaveDelay = 8f;
        [SerializeField] private int _amountOfWaves = 3;

        private Transform[] _spawnPoints;

        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            _spawnPoints = gameObject.GetComponentsInChildren<Transform>();
        }

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Animate.Delay(_spawnStartDelay, SpawnEntitiesAtEverySpawnPoint, true);
        }

        private void SpawnEntitiesAtEverySpawnPoint()
        {
            int i = 0;
            foreach (var spawnPoint in _spawnPoints)
            {
                Animate.Delay(i * Random.Range(0.1f, 0.3f), () => { SpawnEntity(spawnPoint.position); }, true);
                i++;
            }
            _amountOfWaves--;

            if (_amountOfWaves > 0)
            {
                Animate.Delay(_spawnStartDelay, SpawnEntitiesAtEverySpawnPoint, true);
            }
        }

        private void SpawnEntity(Vector3 spawnPos) 
        {
            EnemySpawnPack spawnPack = _enemySpawnPacks[Random.Range(0, 2)];

            NetworkUtils.Instantiate(spawnPack.GetRandomMinion().name, spawnPos);
        }
    }
}
