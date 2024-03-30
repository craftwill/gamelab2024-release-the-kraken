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

            // Attack closest player if close enough
            if (_target != null && _closestPlayerDistance <= _attackRange)
            {
                // Turn towards player before trying to attack
                transform.LookAt(_target.position, Vector3.up);
                TryAttack();
            }
        }

        private void TryAttack()
        {
            void attackLockDoneCallback() 
            {
                if (_navMeshAgent.isOnNavMesh)
                    _navMeshAgent.isStopped = false;
            }

            bool attackLaunched = _entityAttackComponent.TryAttack(_target.position, attackLockDoneCallback);

            if (attackLaunched) 
            {
                // Stop moving during attack
                _navMeshAgent.isStopped = true;
            }
        }
    }
}