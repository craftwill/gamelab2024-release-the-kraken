using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Kraken
{
    public class Spawner : MonoBehaviour
    {
        private List<Transform> _spawnPoints;
        public List<Transform> SpawnPoints
        {
            get
            {
                if(_spawnPoints is null)
                {
                    _spawnPoints = GetComponentsInChildren<Transform>().Where(x => x.CompareTag("SpawnPoint")).ToList();
                }
                return _spawnPoints;
            }
            private set
            {
                _spawnPoints = value;
            }
        }

        public Vector3 GetRandomPosition()
        {
            if (SpawnPoints.Count == 0) return Vector3.zero;
            return SpawnPoints[Random.Range(0, SpawnPoints.Count)].position;
        }
    }
}