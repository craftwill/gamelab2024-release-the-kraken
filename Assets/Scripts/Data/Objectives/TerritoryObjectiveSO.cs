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
            List<Spawner> spawners = new List<Spawner>();
            // Night scaling
            spawnFrequency *= Mathf.Pow(Config.current.objectiveSpawningScaling, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0));

            if (instance.Zones is not null)
            {
                foreach(Zone z in instance.Zones)
                {
                    spawners.Add(z.GetSpawner());
                    z.SetIsActiveZone(true);
                }
                Animate.Repeat(spawnFrequency, () =>
                {
                    bool isInProgress = !instance.IsCompleted;
                    if (isInProgress)
                    {
                        spawners.ForEach(x => NetworkUtils.Instantiate(spawnData.GetRandomEnemy().name, x.GetRandomPosition()));
                    }
                    return isInProgress;
                }, -1, true);
            }
        }

        public override void EndObjective(ObjectiveInstance instance)
        {
            base.EndObjective(instance);

            instance.Zones?.ForEach(x => x?.SetIsActiveZone(false));
            EventManager.Dispatch(EventNames.UpdateCurrentZoneOccupancyUI, new UpdateZoneOccupancyUIData(0, 10));
        }
    }
}