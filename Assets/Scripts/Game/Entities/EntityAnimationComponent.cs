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

            SetLoopedStateIdle();
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

        public void SetLoopedStateIdle()
        {
            _animStateMachine.SetLoopedState(EntityAnimState.Idle, _animatorPrefix, _animatorSuffix);
        }

        public void SetLoopedStateWalking()
        {
            _animStateMachine.SetLoopedState(EntityAnimState.Walk, _animatorPrefix, _animatorSuffix);
        }

        public void PlayBasicAttackAnimation() 
        {
            _animStateMachine.PlayAnimOnce(EntityAnimState.BasicAttack, _animatorPrefix, _animatorSuffix);
        }

        // Animations used by every entity
        public class EntityAnimState : KrakenBaseAnimState
        {
            static public readonly EntityAnimState Idle = new EntityAnimState("Idle", 1f);
            static public readonly EntityAnimState Walk = new EntityAnimState("Walk", 1f);
            static public readonly EntityAnimState Die = new EntityAnimState("Die", 1f);

            // Player's will use a different animState because they use combo attacks.
            static public readonly EntityAnimState BasicAttack = new EntityAnimState("BasicAttack", 1f);

            public EntityAnimState(string pClipName, float pAnimSpeed = 1f, int pNbVariations = -1)
                : base(pClipName, pAnimSpeed, pNbVariations)
            {

            }
        }
    }
}
