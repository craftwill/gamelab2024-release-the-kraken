using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Kraken
{
    public class RangeEnemyController : PathfindingEntityController
    {
        [SerializeField] private NavMeshAgent _agent;

        public override void InitSettings(EnemyConfigSO config)
        {
            base.InitSettings(config);

            if (_agent) _agent.stoppingDistance = _attackRange - 1f;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            
        }
    }
}