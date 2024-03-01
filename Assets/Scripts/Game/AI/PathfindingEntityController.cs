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

        protected Transform _target;
        protected float _pathfindingDistanceRadius;
        protected float _closestDistance = 0f;

        public override void InitSettings(EnemyConfigSO config)
        {
            base.InitSettings(config);
            _pathfindingDistanceRadius = config.pathfindingDistanceRadius;
        }

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
            _closestDistance = closestDistance;
            
            if (closestPlayer == null || _closestDistance > _pathfindingDistanceRadius) 
            {
                _target = null;
                return;
            }

            _target = closestPlayer.transform;

            // Attack closest player if close enough
            if (_closestDistance <= _attackRange)
            {
                _entityAttackComponent.TryAttack(_target);
            }
        }
    }
}