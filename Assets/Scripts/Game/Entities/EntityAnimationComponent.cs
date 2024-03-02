using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Bytes;

namespace Kraken
{
    public class EntityAnimationComponent : MonoBehaviour
    {
        [SerializeField] protected KrakenAnimationStateMachine _animStateMachine;
        [SerializeField] protected string _animatorPrefix = "";
        [SerializeField] protected string _animatorSuffix = "";

        private void Start()
        {
            if (_animStateMachine.GetAnimator() == null) return;

            _animStateMachine.GetAnimator().speed = Random.Range(0.9f, 1.1f);
            
            SetLoopedStateIdle();
        }

        public void PlayHurtAnim()
        {
            if (_animStateMachine.GetAnimator() == null) return;

            _animStateMachine.PlayAnimOnce(EntityAnimState.Hurt, _animatorPrefix, _animatorSuffix);
        }

        public bool SetLoopedStateIdle()
        {
            EntityAnimState newAnimState = EntityAnimState.Idle;
            bool didAnimStateChange = _animStateMachine.GetCurrentLoopedState()?.ClipName != newAnimState.ClipName;
            _animStateMachine.SetLoopedState(newAnimState, _animatorPrefix, _animatorSuffix);

            return didAnimStateChange;
        }

        public bool SetLoopedStateWalking()
        {
            EntityAnimState newAnimState = EntityAnimState.Walk;
            bool didAnimStateChange = _animStateMachine.GetCurrentLoopedState()?.ClipName != newAnimState.ClipName;
            _animStateMachine.SetLoopedState(newAnimState, _animatorPrefix, _animatorSuffix);

            return didAnimStateChange;
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
            static public readonly EntityAnimState Hurt = new EntityAnimState("Hurt", 1f);
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
