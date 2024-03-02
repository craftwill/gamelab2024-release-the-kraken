using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Kraken
{
    public class Spawner : MonoBehaviour
    {
        private List<Transform> spawnPoints;

        private void Start()
        {
            spawnPoints = GetComponentsInChildren<Transform>().Where(x => x.CompareTag("SpawnPoint")).ToList();
        }

        public Vector3 GetRandomPosition()
        {
            if (spawnPoints.Count == 0) return Vector3.zero;
            return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }

        public List<Transform> GetSpawnPoints()
        {
            return spawnPoints;
        }
    }
}