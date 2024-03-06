
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class DazzlePullAbility : MonoBehaviourPun
    {
        private float _damageDealt = 5f;
        private float _radius = 4f;

        private void InitAbility() 
        {
            // Load configs
            _damageDealt = Config.current.dazzleAbilityDamage;
            _radius = Config.current.dazzleAbilityRadius;

            // Size ability mesh approximatly to the radius size
            transform.localScale *= _radius / 4f;
        }

        public void ActivateAbility()
        {
            InitAbility();
            photonView.RPC(nameof(RPC_All_ActivateDazzleAbility), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_All_ActivateDazzleAbility()
        {
            List<EnemyEntity> enemiesFound = CombatUtils.GetEnemyEntitiesInRadius(transform.position, _radius);

            Debug.Log("count: " + enemiesFound.Count);
            foreach (EnemyEntity enemyEntity in enemiesFound)
            {
                Debug.Log(enemyEntity.name);
                enemyEntity.TakeDamage(_damageDealt);
            }

            Animate.Delay(0.35f, () => 
            {
                if (!PhotonNetwork.IsMasterClient) return;

                PhotonNetwork.Destroy(photonView);
            });
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap Sphere to draw your Wire Sphere.
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}