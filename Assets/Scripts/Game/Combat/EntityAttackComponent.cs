using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Bytes;
using Kraken.Network;

namespace Kraken
{
    public class EntityAttackComponent : MonoBehaviourPun
    {
        [SerializeField] private Entity _ownerEntity;
        [SerializeField] private InflictDamageComponent _inflictDamageComponent;
        [SerializeField] private EntityAnimationComponent _entityAnimationComponent;
        [SerializeField] private EnemySoundComponent _soundComponent;

        [Header("Attack Settings")]
        [SerializeField] private float _damageDealt = 1;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _attackDuration = 0.1f;
        [SerializeField] private float _lockedIntoAttackDuration = 0.5f;
        [SerializeField] private GameObject _rangedProjectile = null;

        public bool IsLockedInAttack { get; private set; } = false;
        private bool _canAttack = true;

        public void InitSettings(float dmgDealt, float atkCdr, float atkDuration, float lockedInAtkDuration, GameObject rangedProjectile)
        {
            _damageDealt = dmgDealt;
            _attackCooldown = atkCdr;
            _attackDuration = atkDuration;
            _lockedIntoAttackDuration = lockedInAtkDuration;
            _rangedProjectile = rangedProjectile;
        }

        private void Awake()
        {
            _inflictDamageComponent.Damage = _damageDealt;
            _inflictDamageComponent.Damageclan = _ownerEntity.EntityClan;
        }

        // Returns if an attack has been launched.
        public bool TryAttack(Vector3 targetPosition, System.Action attackLockDoneCallback = null)
        {
            if (!PhotonNetwork.IsMasterClient || !_canAttack) { return false; }

            IsLockedInAttack = true;
            _canAttack = false;
            photonView.RPC(nameof(RPC_All_Attack), RpcTarget.All, targetPosition);

            Animate.Delay(_attackCooldown, () => {
                if (this == null) { return; }
                _canAttack = true;
            }, timeScaled_: true);

            Animate.Delay(_lockedIntoAttackDuration, () => {
                if (this == null) { return; }
                IsLockedInAttack = false;
                attackLockDoneCallback?.Invoke();
            }, timeScaled_: true);

            return true;
        }

        [PunRPC]
        private void RPC_All_Attack(Vector3 targetPosition)
        {
            // All clients play attack animation and collider
            if (!GetUsesRangedAttack())
            {
                MeleeAttack();
            }
            else 
            {
                RangedAttack(targetPosition);
            }
        }

        private void MeleeAttack()
        {
            PlayAttackAnim();
            _inflictDamageComponent.gameObject.SetActive(true);

            // Only master client processes attack duration
            if (!PhotonNetwork.IsMasterClient) return;

            Animate.Delay(_attackDuration, () => {
                if (this == null) { return; }
                _inflictDamageComponent.gameObject.SetActive(false);
            }, timeScaled_: true);
        }

        private void RangedAttack(Vector3 targetPosition)
        {
            // Only master client processes projectile creation
            if (!PhotonNetwork.IsMasterClient) return;

            PlayAttackAnim();

            Vector3 spawnPos = transform.position + transform.forward;
            BaseProjectile projectile = NetworkUtils.Instantiate(_rangedProjectile.name, spawnPos).GetComponent<BaseProjectile>();
            StartCoroutine(DeleteProjectile(2f, projectile));

            IEnumerator DeleteProjectile(float delay, BaseProjectile p)
            {
                yield return new WaitForSeconds(delay);
                if(p && p.gameObject)
                {
                    PhotonNetwork.Destroy(p.gameObject);
                }
            }

            Vector3 sendDirection = (targetPosition - transform.position).normalized;

            // This send() calls a RPC_All behind the scene
            projectile.InitAndSend(sendDirection, _ownerEntity.EntityClan, _damageDealt);
            photonView.RPC(nameof(_soundComponent.RPC_All_PlayEnemyAttackSound), RpcTarget.All);
        }

        private void PlayAttackAnim()
        {
            // If currently playing hurt anim, don't play attack anim.
            if (_entityAnimationComponent.GetCurrentPlayOnceAnim()?.ClipName == "Hurt")
                return;

            _entityAnimationComponent.PlayBasicAttackAnimation();
        }

        private bool GetUsesRangedAttack() 
        {
            return _rangedProjectile != null;
        }
    }
}
