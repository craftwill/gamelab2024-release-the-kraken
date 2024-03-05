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
        [SerializeField] private MinibossAttackAdditionalConfig _attackConfig;

        private Coroutine _drawCoroutine = null;

        //MinibossEntityController does not use EntityAttackComponent
        public override void InitSettings(EnemyConfigSO config)
        {
            base.InitSettings(config);
            _inflictDamageComponent.Damage = config.damageDealt;
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

            if(_target && _closestPlayerDistance <= _attackRange)
            {
                if (_drawCoroutine is null)
                {
                    photonView.RPC(nameof(RPC_ALL_StartConeTelegraph), RpcTarget.All);
                }
            }
        }

        //since the miniboss only has one attack, I use the EnemyConfigSO as part of its attack config with some additional parameters
        [PunRPC]
        private void RPC_ALL_StartConeTelegraph()
        {
            _coneTelegraph.gameObject.SetActive(true);
            _drawCoroutine = StartCoroutine(ConeCharge());
            
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
            _coneTelegraph.DrawCone(1f, true);
            _coneTelegraph.gameObject.SetActive(false);
            _drawCoroutine = null;
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