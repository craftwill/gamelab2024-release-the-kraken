using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class EntityAnimationComponent : MonoBehaviour
    {
        [SerializeField] private KrakenAnimationStateMachine _animStateMachine;
        [SerializeField] private string _animatorPrefix;
        [SerializeField] private string _animatorSuffix;

        private int _hurtStep = 1;

        private void Start()
        {
            if (_animStateMachine.GetAnimator() == null) return;

            _animStateMachine.GetAnimator().speed = Random.Range(0.9f, 1.1f);
        }

        public void PlayHurtAnim()
        {
            if (_animStateMachine.GetAnimator() == null) return;

            _animStateMachine.GetAnimator().Play(_animatorSuffix + "_hurt" + _hurtStep);
            _hurtStep++;
            if (_hurtStep > 2)
            {
                _hurtStep = 1;
            }
        }

        // Animations used by every entity
        public class EntityAnimState : KrakenBaseAnimState
        {
            static public readonly EntityAnimState Idle = new EntityAnimState("Idle", 1f);
            static public readonly EntityAnimState Walk = new EntityAnimState("Walk", 1f);
            static public readonly EntityAnimState Die = new EntityAnimState("Die", 1f);

            public EntityAnimState(string pClipName, float pAnimSpeed = 1f, int pNbVariations = -1)
                : base(pClipName, pAnimSpeed, pNbVariations)
            {

            }
        }
    }
}
