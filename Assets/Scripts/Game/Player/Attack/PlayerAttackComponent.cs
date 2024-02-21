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
        [SerializeField] public bool IsFreeToAttack { get; set; } = true;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                _attacks.ForEach(x => x.Subscribe(this));

                //temporary
                _feedbacks = GetComponent<MoreMountains.Feedbacks.MMF_Player>();
            }
        }
        private void OnDestroy()
        {
            if (_isOwner)
            {
                _attacks.ForEach(x => x.Unsubscribe());
            }
        }
        //Every changes in this file is temporary and is just to showcase
        private MoreMountains.Feedbacks.MMF_Player _feedbacks;
        private void Update()
        {
            /*if (_isOwner)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _feedbacks.PlayFeedbacks();
                }
            }*/
        }
    }
}