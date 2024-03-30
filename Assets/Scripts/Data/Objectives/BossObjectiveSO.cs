using Bytes;
using Kraken.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "BossObjective", menuName = "Kraken/Systems/BossObjective")]
    public class BossObjectiveSO : ObjectiveSO
    {
        [Header("Boss config")]
        public GameObject bossPrefab;

        public override void TriggerObjective(ObjectiveInstance instance)
        {
            base.TriggerObjective(instance);

            EnemyEntity entity = NetworkUtils.Instantiate(bossPrefab.name, instance.Zones[0].GetSpawner().GetRandomPosition()).GetComponent<EnemyEntity>();
            entity.GetHealthComponent().OnDie.AddListener(BossDeath);

            EventManager.Dispatch(EventNames.ShowReinforcementHintUI, new StringDataBytes("THY QUEEN IS HERE!"));
        }

        private void BossDeath()
        {
            EventManager.Dispatch(EventNames.PlayerWin, null);
        }
    }
}