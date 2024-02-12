using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System;

namespace Kraken
{
    public class PlayerAttackComponent : MonoBehaviourPun
    {
        private bool _isOwner;
        [SerializeField] private List<AttackSO> _attacks = new List<AttackSO>();

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                _attacks.ForEach(x => x.Subscribe(this));
            }
        }
        private void OnDestroy()
        {
            if (_isOwner)
            {
                _attacks.ForEach(x => x.Unsubscribe());
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position + this.transform.forward, 1);
            Gizmos.DrawWireSphere(this.transform.position + this.transform.forward + Vector3.up, 1);
        }
    }
}