using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public class RazzlePullAbility : MonoBehaviourPun
    {
        [SerializeField] private float _pullDuration = 6f;
        [SerializeField] private float _radius = 6f;
        [SerializeField] private float _pullStrength = 2f;

        private List<EnemyEntity> _enemyEntitiesBeingPulled = new List<EnemyEntity>();
        private float _timeSinceStart = 0f;
        private bool _isPulling;

        public void ActivateAbility()
        {
            photonView.RPC(nameof(RPC_All_ActivateAbility), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_All_ActivateAbility()
        {
            _isPulling = true;
            _timeSinceStart = 0;

            Collider[] collidersFoundInSphere = Physics.OverlapSphere(transform.position, _radius, Physics.AllLayers, QueryTriggerInteraction.Collide);

            foreach (Collider col in collidersFoundInSphere)
            {
                EnemyEntity enemyEntity = col.GetComponent<EnemyEntity>();
                if (enemyEntity != null)
                {
                    if (_enemyEntitiesBeingPulled.Contains(enemyEntity)) continue;

                    _enemyEntitiesBeingPulled.Add(enemyEntity);
                    PrepareEnemyBeforePull(enemyEntity);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!_isPulling) return;

            foreach (EnemyEntity enemy in _enemyEntitiesBeingPulled)
            {
                Rigidbody rg = enemy.GetComponent<Rigidbody>();
                Vector3 dirToCenter = -(enemy.transform.position - transform.position).normalized;
                float distanceToCenter = Vector3.Distance(enemy.transform.position, transform.position);
                Vector3 pullForce = (dirToCenter * (3f + _timeSinceStart * _timeSinceStart));
                rg.AddForce((pullForce / distanceToCenter) * _pullStrength, ForceMode.Acceleration);
            }

            _timeSinceStart += Time.fixedDeltaTime;
            if (_timeSinceStart >= _pullDuration)
            {
                _isPulling = false;
                foreach (EnemyEntity enemy in _enemyEntitiesBeingPulled)
                {
                    enemy.EnableControllerAndDisablePhysics();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap Sphere to draw your Wire Sphere.
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        private void PrepareEnemyBeforePull(EnemyEntity enemyEntity) 
        {
            enemyEntity.DisableControllerAndEnablePhysics();
        }
    }
}
