using Kraken.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "BossObjective", menuName = "Kraken/Systems/BossObjective")]
    public class BossObjectiveSO : ObjectiveSO
    {
        public GameObject bossPrefab;
        public override void TriggerObjective(ObjectiveInstance instance)
        {
            base.TriggerObjective(instance);

            NetworkUtils.Instantiate(bossPrefab.name, instance.Zone.GetSpawner().GetRandomPosition());
        }
    }
}