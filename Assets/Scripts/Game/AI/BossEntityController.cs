using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class BossEntityController : PathfindingEntityController
    {
        [SerializeField] private RingsOfLightAttack _rolAttack;
        [SerializeField] private StarfallAttack _starfallAttack;
        [SerializeField] private BossAnimationComponent _bossAnim;
        [SerializeField] private Kraken.Game.HealthComponent _healthComponent;

        [SerializeField] private StarfallAttackConfig _starfallConfig;
        [SerializeField] private RingsOfLightAttackConfig _rolConfig;

        private bool _isOnCooldown = false;

        public override void InitSettings(EnemyConfigSO config)
        {
            base.InitSettings(config);
            _healthComponent.OnDie.AddListener(OnDieListener);
        }

        protected override void Start()
        {
            base.Start();

            if (!PhotonNetwork.IsMasterClient) return;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!PhotonNetwork.IsMasterClient) return;

            //A state machine could have been good
            if (!_isOnCooldown)
            {
                _isOnCooldown = true;
                float cd = 0f;
                if(Random.value > 0.5f)
                {
                    _bossAnim.PlayRingsOfLightAttack();
                    _rolAttack.StartAttack(_rolConfig.ring1ChargeTime, _rolConfig.ring2ChargeTime, _rolConfig.ring1Radius, _rolConfig.ring2Radius, _rolConfig.damage);
                    cd = _rolConfig.cooldown;
                    
                }
                else
                {
                    _bossAnim.PlayStarfallAttack();
                    _starfallAttack.StartAttack(_starfallConfig.starChargeTime, _starfallConfig.attackRadius, _starfallConfig.starCount, _starfallConfig.delayBetweenStars, _starfallConfig.telegraphRadius, _starfallConfig.damage);
                    cd = _starfallConfig.cooldown;
                }

                Animate.Delay(cd, () => _isOnCooldown = false, true);
            }
        }

        private void OnDieListener()
        {
            StopAllCoroutines();
            PhotonNetwork.Destroy(_rolAttack.gameObject);
            PhotonNetwork.Destroy(_starfallAttack.gameObject);
        }
    }

    [System.Serializable]
    public struct StarfallAttackConfig
    {
        public float starChargeTime;
        public float attackRadius;
        public int starCount;
        public float delayBetweenStars;
        public float telegraphRadius;
        public int damage;
        public float cooldown;
    }

    [System.Serializable]
    public struct RingsOfLightAttackConfig
    {
        public float ring1ChargeTime;
        public float ring2ChargeTime;      
        public float ring1Radius;
        public float ring2Radius;
        public int damage;
        public float cooldown;
    }
}