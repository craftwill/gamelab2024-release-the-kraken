using Bytes;
using Kraken.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Kraken
{
    [CreateAssetMenu(fileName = "TerritoryObjective", menuName = "Kraken/Systems/TerritoryObjective")]
    public class TerritoryObjectiveSO : ObjectiveSO
    {
        [System.Serializable]
        public class Entry
        {
            public int spawnRatio;
            public GameObject prefab;
        }

        [Header("Territory config")]
        [Tooltip("At what interval do enemies spawn")] public int spawnFrequency;
        [Tooltip("How many total enemies will be spawned")] public int spawnCount;
        [Tooltip("The spawner game object")] public Spawner spawner;
        public List<Entry> spawnData = new List<Entry>();

        public override void TriggerObjective(ObjectiveInstance instance)
        {
            base.TriggerObjective(instance);

            Spawner[] spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.InstanceID);
            spawner = spawners[Random.Range(0, spawners.Length)];

            //should not be kept this way
            Animate.Repeat(spawnFrequency, () =>
            {
                NetworkUtils.Instantiate(GetRandomEnemy().name, spawner.GetRandomPosition());
                return true;
            }, spawnCount, true);

        }

        //Randomly selects an item in the list with the probability based on the weight
        private GameObject GetRandomEnemy()
        {
            int totalWeight = spawnData.Sum(x => x.spawnRatio);
            int rand = Random.Range(0, totalWeight);
            int itemWeightIndex = 0;
            foreach(Entry e in spawnData)
            {
                itemWeightIndex += e.spawnRatio;
                if (rand < itemWeightIndex) return e.prefab;
            }
            return null;
        }
    }
}