using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Photon.Pun;
using MoreMountains.Feedbacks;

namespace Kraken.Game
{
    public class HealthComponent : MonoBehaviourPun
    {
        [SerializeField] private MMF_Player _feedback;
        [field: SerializeField] public float Health { get; private set; } = 10f;
        [field: SerializeField] public float MaxHealth { get; set; } = 10f;
        [field: SerializeField] public bool IsAlive { get; private set; } = true;

        [Header("Testing")]
        public bool _destroyOnDie;

        [Header("Events")]
        public UnityEvent<float> OnTakeDamage;
        public UnityEvent OnDie;

        private DetectDamageComponent[] _detectDmgComps;

        private void Awake()
        {
            Health = MaxHealth;

            _detectDmgComps = GetComponentsInChildren<DetectDamageComponent>();
            foreach (var detectDmgComp in _detectDmgComps)
            {
                detectDmgComp.OnDetectDamage.AddListener(TakeDamage);
            }
        }

        private void OnDestroy()
        {
            foreach (var detectDmgComp in _detectDmgComps)
            {
                detectDmgComp.OnDetectDamage.RemoveListener(TakeDamage);
            }
        }

        public void TakeDamage(float dmgAmount) 
        {
            photonView.RPC(nameof(RPC_Master_TakeDamage), RpcTarget.All, dmgAmount);
        }

        [PunRPC]
        private void RPC_Master_TakeDamage(float dmgAmount)
        {
            if (!IsAlive) return;

            Health -= dmgAmount;
            OnTakeDamage.Invoke(dmgAmount);
            _feedback.PlayFeedbacks();

            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
        }

        private void Die()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            IsAlive = false;
            OnDie.Invoke();

            if (_destroyOnDie)
            {
                PhotonNetwork.Destroy(photonView);
            }
        }
    }
}
