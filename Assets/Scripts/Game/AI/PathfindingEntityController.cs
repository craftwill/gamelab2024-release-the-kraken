using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Kraken 
{
    public class PathfindingEntityController : BaseEntityController
    {
        [SerializeField] protected Entity _ownerEntity;
        [SerializeField] protected EntityAttackComponent _entityAttackComponent;
        [SerializeField] protected NavMeshAgent _navMeshAgent;

        [SerializeField] protected float _attackDistance = 2.5f;

        protected Transform _target;

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if (!PhotonNetwork.IsMasterClient) return;

            if (!_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.enabled = false;
                enabled = false;
                return;
            }

            if (_target == null) return;

            Vector3 destination = _target.position;
            _navMeshAgent.SetDestination(destination);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!PhotonNetwork.IsMasterClient) { return; }

            if (_entityAttackComponent.IsAttacking) return;

            (PlayerEntity closestPlayer, float closestDistance) = _ownerEntity.GetClosestPlayer();
            _target = closestPlayer.transform;

            // Attack closest player if close enough
            if (closestDistance <= _attackDistance)
            {
                _entityAttackComponent.TryAttack();
            }
        }
    }
}