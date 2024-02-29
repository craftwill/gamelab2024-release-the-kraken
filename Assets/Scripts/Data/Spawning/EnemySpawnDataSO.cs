using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "EnemySpawnData", menuName = "Kraken/Spawning/EnemySpawnData")]
    public class EnemySpawnDataSO : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public int spawnRatio;
            public GameObject prefab;
        }

        public List<Entry> spawnData;

        //Randomly selects an item in the list with the probability based on the weight
        public GameObject GetRandomEnemy()
        {
            int totalWeight = spawnData.Sum(x => x.spawnRatio);
            int rand = Random.Range(0, totalWeight);
            int itemWeightIndex = 0;
            foreach (Entry e in spawnData)
            {
                itemWeightIndex += e.spawnRatio;
                if (rand < itemWeightIndex) return e.prefab;
            }
            return null;
        }
    }
}