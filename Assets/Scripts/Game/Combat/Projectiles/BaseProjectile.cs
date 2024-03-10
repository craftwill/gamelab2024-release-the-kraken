using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public class BaseProjectile : MonoBehaviourPun
    {
        [field: SerializeField] public InflictDamageComponent InflictDamageComponent { private set; get; }
        public bool CanDamage { get; private set; } = true;
        protected Vector3 _currentDirection;
        protected float _speed = 0.33f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground")) 
            {
                ProjectileHit();
            }
        }

        public virtual void InitAndSend(Vector3 direction, EntityClan damageClan, float damage)
        {
            InflictDamageComponent.Damageclan = damageClan;
            InflictDamageComponent.Damage = damage;
            photonView.RPC(nameof(RPC_All_Send), RpcTarget.All, direction);
        }

        [PunRPC]
        protected virtual void RPC_All_Send(Vector3 direction) 
        {
            _currentDirection = direction;
        }

        public void ProjectileHit() 
        {
            photonView.RPC(nameof(RPC_All_ProjectileHit), RpcTarget.All);
        }

        [PunRPC]
        protected virtual void RPC_All_ProjectileHit()
        {
            CanDamage = false;
            // Play projectile VFX here

            if (!photonView.AmOwner) return;

            PhotonNetwork.Destroy(photonView);
        }
    }
}
