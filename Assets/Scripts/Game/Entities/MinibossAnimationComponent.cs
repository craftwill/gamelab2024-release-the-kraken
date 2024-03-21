using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MinibossAnimationComponent : EntityAnimationComponent
    {
        public void PlayMinibossAttack(System.Action animDonePlayingCallback = null)
        {
            MinibossAnimState attackComboState = new("Attack", 1f);
            _animStateMachine.PlayAnimOnce(attackComboState, _animatorPrefix, _animatorSuffix, "", 1f, animDonePlayingCallback);
        }

        // Animations used by the miniboss
        public class MinibossAnimState : EntityAnimState
        {
            public MinibossAnimState(string pClipName, float pAnimSpeed = 1f, int pNbVariations = -1)
                : base(pClipName, pAnimSpeed, pNbVariations)
            {

            }
        }
    }
}