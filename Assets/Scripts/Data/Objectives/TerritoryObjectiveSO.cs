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
            // Night scaling
            spawnFrequency *= Mathf.Pow(Config.current.objectiveSpawningScaling, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0));

            if (instance.Zones is not null)
            {
                foreach(Zone z in instance.Zones)
                {
                    z.SetIsActiveZone(true);

                    if (z.ZoneHasTower()) spawnFrequency *= Config.current.towerSpawnMultiplier;

                    Animate.Repeat(spawnFrequency, () =>
                    {
                        bool isInProgress = !instance.IsCompleted;
                        if (isInProgress)
                        {
                            NetworkUtils.Instantiate(spawnData.GetRandomEnemy().name, z.GetSpawner().GetRandomPosition());
                        }
                        return isInProgress;
                    }, -1, true);
                }            
            }

        }
    }
}