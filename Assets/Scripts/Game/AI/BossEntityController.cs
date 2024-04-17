
using UnityEngine;

using Bytes;

using Photon.Pun;

namespace Kraken
{
    public class BossEntityController : PathfindingEntityController
    {
        [SerializeField] private RingsOfLightAttack _rolAttack;
        [SerializeField] private StarfallAttack _starfallAttack;
        [SerializeField] private BossAnimationComponent _bossAnim;
        [SerializeField] private Game.HealthComponent _healthComponent;
        [SerializeField] private BossSoundComponent _soundComponent;

        [SerializeField] private StarfallAttackConfig _starfallConfig;
        [SerializeField] private RingsOfLightAttackConfig _rolConfig;

        private bool _isOnCooldown = false;

        private Transform _rootMesh;
        private PlayerEntity _playerRef;

        public override void InitSettings(EnemyConfigSO config)
        {
            base.InitSettings(config);
            _healthComponent.OnTakeDamage.AddListener(OnTakeDamageListener);
            _healthComponent.OnDie.AddListener(OnDieListener);
        }

        protected override void Start()
        {
            base.Start();
            _rootMesh = GetComponentInChildren<Animator>().transform;
            _playerRef = CombatUtils.GetPlayerEntities()[0];
            // When boss spawns, show its HP bar
            EventManager.Dispatch(EventNames.UpdateBossHealthUI, new UpdateBossHealthUIData(isMiniBoss: false, 1f));
            if (!PhotonNetwork.IsMasterClient) return;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            //_target is only set on master, this always looks at the same player
            _rootMesh.LookAt(new Vector3(_playerRef.transform.position.x, _rootMesh.position.y, _playerRef.transform.position.z), Vector3.up);

            if (!PhotonNetwork.IsMasterClient) return;

            //A state machine could have been good
            if (!_isOnCooldown)
            {
                _isOnCooldown = true;
                float cd = 0f;
                if(Random.value > 0.5f)
                {
                    photonView.RPC(nameof(RPC_ALL_PlayRoLAnim), RpcTarget.All);
                    _rolAttack.StartAttack(_rolConfig.ring1ChargeTime, _rolConfig.ring2ChargeTime, _rolConfig.ring1Radius, _rolConfig.ring2Radius, _rolConfig.damage);
                    cd = _rolConfig.cooldown;
                    
                }
                else
                {
                    photonView.RPC(nameof(RPC_ALL_PlayStarfallAnim), RpcTarget.All);
                    _starfallAttack.StartAttack(_starfallConfig.starChargeTime, _starfallConfig.attackRadius, _starfallConfig.starCount, _starfallConfig.delayBetweenStars, _starfallConfig.telegraphRadius, _starfallConfig.damage);
                    cd = _starfallConfig.cooldown;
                }

                Animate.Delay(cd, () => _isOnCooldown = false, true);
            }
        }

        [PunRPC]
        private void RPC_ALL_PlayStarfallAnim()
        {
            //not sure if it works
            void PlayFuckingIdle()
            {
                _bossAnim.SetLoopedStateIdle();
            }
            _bossAnim.PlayStarfallAttack(PlayFuckingIdle);
        }

        [PunRPC]
        private void RPC_ALL_PlayRoLAnim()
        {
            //not sure if it works
            void PlayFuckingIdle()
            {
                _bossAnim.SetLoopedStateIdle();
            }
            _bossAnim.PlayRingsOfLightAttack(PlayFuckingIdle);
        }

        private void OnTakeDamageListener(float dmgAmount)
        {
            bool isShowUltIndicator = _healthComponent.Health <= Config.current.ultimateDamage;
            float fillAmount = _healthComponent.Health / _healthComponent.MaxHealth;
            EventManager.Dispatch(EventNames.UpdateBossHealthUI, new UpdateBossHealthUIData(isMiniBoss: false, fillAmount, isShowUltIndicator));
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