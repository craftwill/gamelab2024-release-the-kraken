using Bytes;
using Kraken.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "TerritoryObjective", menuName = "Kraken/Systems/TerritoryObjective")]
    public class TerritoryObjectiveSO : ObjectiveSO
    {
        [Header("Territory config")]
        [Tooltip("At what interval do enemies spawn")] public int spawnFrequency;
        [Tooltip("How many total enemies will be spawned")] public int spawnCount;
        [Tooltip("The spawner game object")] public Spawner spawner;
        public EnemySpawnData spawnConfig;

        public override void TriggerObjective(ObjectiveInstance instance)
        {
            base.TriggerObjective(instance);

            Spawner[] spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.InstanceID);
            spawner = spawners[0];

            Animate.Repeat(spawnFrequency, () =>
            {
                NetworkUtils.Instantiate(spawnConfig.GetRandomEnemyToSpawn().name, spawner.GetRandomPosition());
                return true;
            }, spawnCount);

        }
    }
}