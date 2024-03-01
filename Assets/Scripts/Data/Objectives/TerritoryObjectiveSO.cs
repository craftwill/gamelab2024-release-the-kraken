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
        [Header("Territory config")]
        [Tooltip("At what interval do enemies spawn")] public float spawnFrequency;
        [SerializeField] private EnemySpawnDataSO spawnData;

        public override void TriggerObjective(ObjectiveInstance instance)
        {
            base.TriggerObjective(instance);
            Spawner spawner = instance.Zone.GetSpawner();

            //should not be kept this way
            Animate.Repeat(spawnFrequency, () =>
            {
                bool isInProgress = !instance.IsCompleted;
                if (isInProgress) NetworkUtils.Instantiate(spawnData.GetRandomEnemy().name, spawner.GetRandomPosition());
                return isInProgress;
            }, -1, true);
        }
    }
}