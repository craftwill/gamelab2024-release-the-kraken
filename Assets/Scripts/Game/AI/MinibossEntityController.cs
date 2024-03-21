using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MinibossEntityController : PathfindingEntityController
    {
        [SerializeField] private ConeTelegraph _coneTelegraph;
        [SerializeField] private InflictDamageComponent _inflictDamageComponent;//could probably avoid using this component
        [SerializeField] private MinibossAnimationComponent _animComponent;
        [SerializeField] private MinibossAttackAdditionalConfig _attackConfig;

        private Coroutine _drawCoroutine = null;
        private float _cooldown = 0f;
        private bool _isOnCooldown = false;

        private bool _canPathfind = true;

        //MinibossEntityController does not use EntityAttackComponent
        public override void InitSettings(EnemyConfigSO config)
        {
            base.InitSettings(config);
            _inflictDamageComponent.Damage = config.damageDealt;
            _cooldown = config.attackCooldown;
        }

        protected override void Start()
        {
            base.Start();

            _coneTelegraph.InitSettings(_attackRange, _attackConfig.angle, _attackConfig.obstructingLayer, _attackConfig.playerLayer, _attackConfig.materialInner, _attackConfig.materialOuter);

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

            if(_target && !_isOnCooldown && _closestPlayerDistance <= _attackRange)
            {
                if (_drawCoroutine is null)
                {
                    _canPathfind = false;
                    _isOnCooldown = true;
                    photonView.RPC(nameof(RPC_ALL_StartConeTelegraph), RpcTarget.All);
                }
            }
        }

        //since the miniboss only has one attack, I use the EnemyConfigSO as part of its attack config with some additional parameters
        [PunRPC]
        private void RPC_ALL_StartConeTelegraph()
        {
            void AnimDonePlayingCallback()
            {
                _animComponent.SetLoopedStateCharged();
            }

            _coneTelegraph.gameObject.SetActive(true);
            _drawCoroutine = StartCoroutine(ConeCharge());
            _animComponent.PlayMinibossPrepareAttack(AnimDonePlayingCallback);
        }

        private IEnumerator ConeCharge()
        {
            float time = 0f;
            float timeToCharge = _attackConfig.chargeTime;
            while(time < timeToCharge)
            {
                _coneTelegraph.DrawCone(time / timeToCharge);
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }
            _animComponent.PlayMinibossAttack();
            _coneTelegraph.DrawCone(1f, true);
            _coneTelegraph.gameObject.SetActive(false);
            _drawCoroutine = null;
            _canPathfind = true;
            Animate.Delay(_cooldown, () => _isOnCooldown = false, true);

        }

        protected override bool CanPathfind()
        {
            return _canPathfind;
        }
    }

    [System.Serializable]
    public struct MinibossAttackAdditionalConfig
    {
        public float chargeTime;
        public float angle;
        public LayerMask obstructingLayer;//layers that block the cone
        public LayerMask playerLayer;
        public Material materialInner;
        public Material materialOuter;
    }
}