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
        protected bool _canMove = true;
        private bool _staggered = false;
        private Coroutine _staggerCoroutine;

        protected Transform _target;
        protected float _roamingDestinationRefreshTime = 0;
        protected float _spawnTime = 0;
        protected Vector3 _lastPos = Vector3.zero;
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
            _spawnTime = Time.time;
        }

        protected override void Update()
        {
            base.Update();

            if (!PhotonNetwork.IsMasterClient || !_isActive || _staggered) return;

            if (!_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.enabled = false;
                enabled = false;
                return;
            }

            if (CanPathfind())
            {
                _navMeshAgent.isStopped = false;
            }
            else
            {
                _navMeshAgent.isStopped = true;
                return;
            }

            if (_lastPos == transform.position) 
            {
                photonView.RPC(nameof(RPC_All_SetLoopedStateIdle), RpcTarget.All);
            }
            else
            {
                photonView.RPC(nameof(RPC_All_SetLoopedStateWalking), RpcTarget.All);
            }
            _lastPos = transform.position;

            if (_target == null)
            {
                if (Time.time - _spawnTime > _roamingDestinationRefreshTime)
                {
                    _navMeshAgent.SetDestination(PickRandomNearbyPoint());
                    _roamingDestinationRefreshTime += Random.Range(Config.current.enemyRoamMinChangeFrequency, Config.current.enemyRoamMaxChangeFrequency);
                }
                return;
            }

            Vector3 destination = _target.position;
            _navMeshAgent.SetDestination(destination);
        }

        [PunRPC]
        private void RPC_All_SetLoopedStateIdle()
        {
            _entityAnimationComponent.SetLoopedStateIdle();
        }

        [PunRPC]
        private void RPC_All_SetLoopedStateWalking()
        {
            _entityAnimationComponent.SetLoopedStateWalking();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!PhotonNetwork.IsMasterClient || !_isActive) return;

            if (!_canMove) return;

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
            _navMeshAgent.ResetPath();
            _entityAnimationComponent.SetLoopedStateIdle(); // Should be hurt but we don't have that
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

            if (_navMeshAgent.isOnNavMesh)
                _navMeshAgent.isStopped = !isActive;
            _navMeshAgent.enabled = isActive;
            _entityAnimationComponent.SetLoopedStateIdle(); // For some reason doesn't seem to work
        }

        protected virtual bool CanPathfind()
        {
            return true;
        }

        private Vector3 PickRandomNearbyPoint()
        {
            Vector2 direction = Random.insideUnitCircle.normalized * Random.Range(Config.current.enemyMinRoamDistance, Config.current.enemyMaxRoamDistance);
            Transform point = transform;
            return new Vector3(point.position.x + direction.x, point.position.y, point.position.z + direction.y);
        }
    }
}