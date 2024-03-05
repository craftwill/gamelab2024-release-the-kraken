using Kraken.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "MinibossObjective", menuName = "Kraken/Systems/MinibossObjective")]
    public class MinibossObjective : ObjectiveSO
    {
        [Header("Miniboss config")]
        public GameObject minibossPrefab;
        [Header("")]
        public TerritoryObjectiveSO territory;

        private ObjectiveInstance _instance;

        public override void TriggerObjective(ObjectiveInstance instance)
        {
            _instance = instance;
            EnemyEntity entity = NetworkUtils.Instantiate(minibossPrefab.name, _instance.Zone.GetSpawner().GetRandomPosition()).GetComponent<EnemyEntity>();
            entity.GetHealthComponent().OnDie.AddListener(MinibossDeath);

            territory.TriggerObjective(_instance);
        }

        public void MinibossDeath()
        {
            _instance.EndObjective(true);
        }
    }
}