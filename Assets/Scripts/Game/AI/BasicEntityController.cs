using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class BasicEntityController : PathfindingEntityController
    {
        [SerializeField] protected EntityAttackComponent _entityAttackComponent;

        protected override void Start()
        {
            base.Start();

            if (!PhotonNetwork.IsMasterClient) return;
        }

        protected override void Update()
        {
            base.Update();

            if (!PhotonNetwork.IsMasterClient) return;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!PhotonNetwork.IsMasterClient) { return; }

            if (_entityAttackComponent.IsAttacking) return;

            // Attack closest player if close enough
            if (_target != null && _closestPlayerDistance <= _attackRange)
            {
                _entityAttackComponent.TryAttack(_target.position);
            }
        }
    }
}