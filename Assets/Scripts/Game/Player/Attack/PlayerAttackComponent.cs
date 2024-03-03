
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public class PlayerAttackComponent : MonoBehaviourPun
    {
        private bool _isOwner;
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private List<AttackSO> _attacks = new List<AttackSO>();
        [SerializeField] public bool IsFreeToAttack { get; set; } = true;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                _attacks.ForEach(x => x.Subscribe(this, _playerEntity));
            }
        }

        private void OnDestroy()
        {
            if (_isOwner)
            {
                _attacks.ForEach(x => x.Unsubscribe());
            }
        }

        public void UnsubscribeAttacks() 
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                _attacks.ForEach(x => x.Unsubscribe());
            }
        }
    }
}