
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class DazzleAoEAbility : MonoBehaviourPun
    {
        [SerializeField] private float _abilityCircleLifeTime = 0.28f;
        [SerializeField] private float _radius = 4f;
        private float _damageDealt = 5f;

        private void InitAbility() 
        {
            // Load configs
            _damageDealt = Config.current.dazzleAbilityDamage;
            _radius = Config.current.dazzleAbilityRadius;

            // Size ability mesh to the radius size (yup, divided by 4 is the way)
            transform.localScale *= _radius / 4f;
        }

        public void ActivateAbility()
        {
            photonView.RPC(nameof(RPC_All_ActivateDazzleAbility), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_All_ActivateDazzleAbility()
        {
            InitAbility();
            List<EnemyEntity> enemiesFound = CombatUtils.GetEnemyEntitiesInRadius(transform.position, _radius);

            foreach (EnemyEntity enemyEntity in enemiesFound)
            {
                enemyEntity.TakeDamage(_damageDealt);
            }

            Animate.Delay(_abilityCircleLifeTime, () => 
            {
                if (!photonView.AmOwner) return;

                PhotonNetwork.Destroy(photonView);
            }, timeScaled_: true);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap Sphere to draw your Wire Sphere.
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}