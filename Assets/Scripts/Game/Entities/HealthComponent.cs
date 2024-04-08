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
        [SerializeField] private FlickerVFX _flickerVFX;
        [field: SerializeField] public float Health { get; private set; } = 10f;
        [field: SerializeField] public float MaxHealth { get; set; } = 10f;
        [field: SerializeField] public bool IsAlive { get; private set; } = true;
        [field: SerializeField] public bool ScaleHpWithNight { get; private set; } = true;

        [Header("Testing")]
        public bool _destroyOnDie;

        [Header("Events")]
        public UnityEvent<float> OnTakeDamage;
        public UnityEvent<float> OnHealed;
        public UnityEvent OnDie;

        private void Start()
        {
            if (ScaleHpWithNight)
            {
                MaxHealth *= Mathf.Pow(Config.current.enemyHealthScaling, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0));
                MaxHealth = Mathf.Round(MaxHealth);
                
            }
            Health = MaxHealth;
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
            _feedback?.PlayFeedbacks();
            _flickerVFX?.PlayHurtFeedback();

            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
        }

        public void GetHealed(float healAmount)
        {
            photonView.RPC(nameof(RPC_MasterGetHealed), RpcTarget.All, healAmount);
        }

        [PunRPC]
        private void RPC_MasterGetHealed(float healAmount)
        {
            OnHealed.Invoke(healAmount);
            if (!IsAlive) return;
            Health = Mathf.Clamp(Health + healAmount, 0, MaxHealth);
        }

        private void Die()
        {
            IsAlive = false;
            if (!PhotonNetwork.IsMasterClient) return;
            
            OnDie.Invoke();

            if (_destroyOnDie)
            {
                PhotonNetwork.Destroy(photonView);
            }
        }
    }
}
