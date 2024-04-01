using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Bytes;
using Kraken.Network;

namespace Kraken
{
    public class PermanentSpawning : MonoBehaviourPun
    {
        [SerializeField] private EnemySpawnDataSO spawnData;
        [SerializeField, Range(0f, 10f)] private float spawnFrequency;
        [SerializeField, Tooltip("How often enemies are spawned in zones vs outside of zones"), Range(0f, 1f)] private float zoneSpawnRatio = 1f;
        [SerializeField, Tooltip("Spawn points that are outside dedicated zones")] private List<Transform> _additionalSpawnpoints;

        private Zone[] _zones;
        private List<Transform> _zoneSpawnPoints = new List<Transform>();
        private bool _spawnActive = false;

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            _zones = FindObjectsByType<Zone>(FindObjectsSortMode.None);

            if (_zones is not null) System.Array.ForEach(_zones, x => _zoneSpawnPoints.AddRange(x.GetSpawner().GetSpawnPoints()));

            EventManager.AddEventListener(EventNames.StartSpawning, HandleStartSpawning);
            EventManager.AddEventListener(EventNames.StopSpawning, HandleStopSpawning);

            // Night scaling
            spawnFrequency *= Mathf.Pow(Config.current.permanentSpawningScaling, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0));
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.StartSpawning, HandleStartSpawning);
            EventManager.RemoveEventListener(EventNames.StopSpawning, HandleStopSpawning);
        }

        private void HandleStartSpawning(BytesData bytes)
        {
            _spawnActive = true;
            
            Animate.Repeat(spawnFrequency, () =>
            {
                Vector3 spawnPos = GetSpawnPoint().position;
                spawnPos += new Vector3(Random.Range(0f, 1f), 0f, Random.Range(0f, 1f)); // Add random offset
                if (_spawnActive) NetworkUtils.Instantiate(spawnData.GetRandomEnemy().name, spawnPos);
                return _spawnActive;

            }, -1, true);
        }

        private void HandleStopSpawning(BytesData bytes)
        {
            _spawnActive = false;
        }

        private Transform GetSpawnPoint()
        {
            if (Random.Range(0f, 1f) < zoneSpawnRatio) return _zoneSpawnPoints[Random.Range(0, _zoneSpawnPoints.Count)];
            else return _additionalSpawnpoints[Random.Range(0, _additionalSpawnpoints.Count)];
        }
    }
}