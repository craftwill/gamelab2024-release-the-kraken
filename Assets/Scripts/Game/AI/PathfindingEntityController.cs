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
        [SerializeField] protected EntityAnimationComponent _entityAnimationComponent;
        [SerializeField] protected NavMeshAgent _navMeshAgent;
        private bool _staggered = false;
        private Coroutine _staggerCoroutine;

        protected Transform _target;
        protected float _pathfindingDistanceRadius;
        protected float _closestPlayerDistance;

        public override void InitSettings(EnemyConfigSO config)
        {
            base.InitSettings(config);
            _pathfindingDistanceRadius = config.pathfindingDistanceRadius;
        }

        protected override void Start()
        {
            base.Start();

            _entityAnimationComponent.SetLoopedStateIdle();
            _navMeshAgent.speed *= _moveSpeed;
        }

        protected override void Update()
        {
            base.Update();

            if (!PhotonNetwork.IsMasterClient || !_isActive) return;

            if (!_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.enabled = false;
                enabled = false;
                return;
            }

            if (_target == null)
            {
                _entityAnimationComponent.SetLoopedStateIdle();
                return;
            }

            _entityAnimationComponent.SetLoopedStateWalking();

            Vector3 destination = _target.position;
            _navMeshAgent.SetDestination(destination);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!PhotonNetwork.IsMasterClient || !_isActive) return;

            (PlayerEntity closestPlayer, float closestDistance) = _ownerEntity.GetClosestPlayer();
            
            if (closestPlayer == null || closestDistance > _pathfindingDistanceRadius || _staggered) 
            {
                _target = null;
                return;
            }

            _closestPlayerDistance = closestDistance;
            _target = closestPlayer.transform;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void Stagger()
        {
            if (_staggerCoroutine == null)
            {
                _staggerCoroutine = StartCoroutine(StaggerCoroutine());
            }
        }

        private IEnumerator StaggerCoroutine()
        {
            _staggered = true;
            _navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(Config.current.enemyStaggerDuration);
            _staggered = false;
            // Will sometimes be disabled due to other game mechanics
            if(_navMeshAgent.enabled)
                _navMeshAgent.isStopped = false;
            _staggerCoroutine = null;
        }

        public override void SetControllerActive(bool isActive)
        {
            base.SetControllerActive(isActive);

            _navMeshAgent.isStopped = !isActive;
            _navMeshAgent.enabled = isActive;
        }
    }
}