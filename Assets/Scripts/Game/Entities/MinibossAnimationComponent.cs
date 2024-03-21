using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MinibossAnimationComponent : EntityAnimationComponent
    {
        public void PlayMinibossPrepareAttack(System.Action animDonePlayingCallback = null)
        {
            MinibossAnimState prepareState = MinibossAnimState.ChargeAttack;
            _animStateMachine.PlayAnimOnce(prepareState, _animatorPrefix, _animatorSuffix, "", 1f, animDonePlayingCallback);
        }

        public bool SetLoopedStateCharged()
        {
            MinibossAnimState chargeState = MinibossAnimState.PrepareAttack;
            bool didAnimStateChange = _animStateMachine.GetCurrentLoopedState()?.ClipName != chargeState.ClipName;
            _animStateMachine.SetLoopedState(chargeState, _animatorPrefix, _animatorSuffix);

            return didAnimStateChange;
        }

        public void PlayMinibossAttack(System.Action animDonePlayingCallback = null)
        {
            MinibossAnimState attackState = MinibossAnimState.Attack;
            _animStateMachine.PlayAnimOnce(attackState, _animatorPrefix, _animatorSuffix, "", 1f, animDonePlayingCallback);
        }

        // Animations used by the miniboss
        public class MinibossAnimState : EntityAnimState
        {
            static public readonly MinibossAnimState Attack = new MinibossAnimState("Attack", 1f);
            static public readonly MinibossAnimState ChargeAttack = new MinibossAnimState("ChargeAttack", 1f);
            static public readonly MinibossAnimState PrepareAttack = new MinibossAnimState("PrepareAttack", 1f);
            public MinibossAnimState(string pClipName, float pAnimSpeed = 1f, int pNbVariations = -1)
                : base(pClipName, pAnimSpeed, pNbVariations)
            {

            }
        }
    }
}