using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class RazzlePullAbility : MonoBehaviourPun
    {
        private float _pullDuration = 5f;
        private float _radius = 4f;
        private float _initialPullStrength = 2f;
        private float _pullStrengthOverTime = 2f;

        private List<EnemyEntity> _enemyEntitiesBeingPulled = new List<EnemyEntity>();
        private float _timeSinceStart = 0f;
        private bool _isPulling;

        private float _initialEnemiesDrag = -1;

        private void InitAbility()
        {
            _pullDuration = Config.current.razzleAbilityPullDuration;
            _radius = Config.current.razzleAbilityRadius;
            _initialPullStrength = Config.current.razzleAbilityInitialPullStrength;
            _pullStrengthOverTime = Config.current.razzleAbilityPullStrengthOverTime;

            // Size ability mesh approximatly to the radius size
            transform.localScale *= _radius / 4f;
        }

        public void ActivateAbility()
        {
            InitAbility();
            photonView.RPC(nameof(RPC_All_ActivateRazzleAbility), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_All_ActivateRazzleAbility()
        {
            _isPulling = true;
            _timeSinceStart = 0;

            FindNewTargets();
        }

        private void FindNewTargets() 
        {
            List<EnemyEntity> enemiesFound = CombatUtils.GetEnemyEntitiesInRadius(transform.position, _radius);

            foreach (EnemyEntity enemyEntity in enemiesFound)
            {
                if (enemyEntity != null)
                {
                    if (_enemyEntitiesBeingPulled.Contains(enemyEntity)) continue;

                    // Get any enemy drag value to later restore it to every enemies after the pull is done.
                    if (Mathf.Approximately(_initialEnemiesDrag, -1f))
                    {
                        _initialEnemiesDrag = enemyEntity.GetComponent<Rigidbody>().drag;
                    }

                    _enemyEntitiesBeingPulled.Add(enemyEntity);
                    PrepareEnemyBeforePull(enemyEntity);
                    PullEnemy(enemyEntity, _initialPullStrength, ForceMode.Impulse);
                }
            }
        }

        private void PrepareEnemyBeforePull(EnemyEntity enemyEntity)
        {
            enemyEntity.DisableControllerAndEnablePhysics();
        }

        private void FixedUpdate()
        {
            if (!_isPulling) return;

            FindNewTargets();
            PullAllEnemies(_pullStrengthOverTime);

            _timeSinceStart += Time.fixedDeltaTime;
            if (_timeSinceStart >= _pullDuration)
            {
                _isPulling = false;
                foreach (EnemyEntity enemy in _enemyEntitiesBeingPulled)
                {
                    enemy.GetComponent<Rigidbody>().drag = _initialEnemiesDrag; // Restore initial drag
                    enemy.EnableControllerAndDisablePhysics();
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(photonView);
                }
            }
        }

        private void PullAllEnemies(float strengthMultiplier, ForceMode forceMode = ForceMode.Acceleration)
        {
            foreach (EnemyEntity enemy in _enemyEntitiesBeingPulled)
            {
                PullEnemy(enemy, strengthMultiplier, forceMode);
            }
        }

        private void PullEnemy(EnemyEntity enemy, float strengthMultiplier, ForceMode forceMode = ForceMode.Acceleration)
        {
            if (enemy == null) return;

            Rigidbody rg = enemy.GetComponent<Rigidbody>();
            Vector3 dirToCenter = -(enemy.transform.position - transform.position).normalized;
            float distanceToCenter = Vector3.Distance(enemy.transform.position, transform.position);
            Vector3 pullForce = dirToCenter;
            Vector3 finalForce = (pullForce / distanceToCenter) * strengthMultiplier;
            rg.AddForce(finalForce, forceMode);
            // Drag stronger the longer time advances and the closer to center
            rg.drag = Mathf.Clamp01(
                Mathf.Lerp(_initialEnemiesDrag, 1f, 0.5f / distanceToCenter)
              + Mathf.Lerp(_initialEnemiesDrag, 1f, _timeSinceStart / _pullDuration)
            );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap Sphere to draw your Wire Sphere.
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}