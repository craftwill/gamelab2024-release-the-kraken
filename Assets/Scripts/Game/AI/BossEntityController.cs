using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class BossEntityController : PathfindingEntityController
    {
        [SerializeField] private StarfallAttackConfig starfallConfig;

        public override void InitSettings(EnemyConfigSO config)
        {
            base.InitSettings(config);
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
        }
    }

    [System.Serializable]
    public struct StarfallAttackConfig
    {
        float chargeTime;
        float attackRadius;
        float starCount;
        float delayBetweenStars;
        float telegraphRadius;
        
        
    }
}