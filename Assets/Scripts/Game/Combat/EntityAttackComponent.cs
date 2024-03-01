using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class EntityAttackComponent : MonoBehaviourPun
    {
        [SerializeField] private Entity _ownerEntity;
        [SerializeField] private InflictDamageComponent _inflictDamageComponent;

        [Header("Attack Settings")]
        [SerializeField] private float _damageDealt = 1;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _attackDuration = 0.1f;
        [SerializeField] private float _lockedIntoAttackDuration = 0.5f;

        public bool IsAttacking { get; private set; } = false;
        private bool _canAttack = true;

        public void InitSettings(float dmgDealt, float atkCdr, float atkDuration, float lockedInAtkDuration, bool isRange = false)
        {
            _damageDealt = dmgDealt;
            _attackCooldown = atkCdr;
            _attackDuration = atkDuration;
            _lockedIntoAttackDuration = lockedInAtkDuration;
        }

        private void Awake()
        {
            _inflictDamageComponent.Damage = _damageDealt;
            _inflictDamageComponent.Damageclan = _ownerEntity.EntityClan;
        }

        public void TryAttack(Transform target) 
        {
            if (!PhotonNetwork.IsMasterClient || !_canAttack) { return; }

            IsAttacking = true;
            _canAttack = false;
            photonView.RPC(nameof(RPC_All_Attack), RpcTarget.All);

            Animate.Delay(_attackCooldown, () => {
                if (this == null) { return; }
                _canAttack = true;
            });

            Animate.Delay(_lockedIntoAttackDuration, () => {
                if (this == null) { return; }
                IsAttacking = false;
            });
        }

        [PunRPC]
        private void RPC_All_Attack() 
        {
            // All clients play attack animation and collider
            _inflictDamageComponent.gameObject.SetActive(true);

            // Only master client processes attack duration
            if (!PhotonNetwork.IsMasterClient) return;

            Animate.Delay(_attackDuration, () => {
                if (this == null) { return; }
                _inflictDamageComponent.gameObject.SetActive(false);
            });
        }
    }
}
