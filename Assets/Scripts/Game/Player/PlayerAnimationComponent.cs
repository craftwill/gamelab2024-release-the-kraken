using Kraken;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerAnimationComponent : EntityAnimationComponent
    {
        [SerializeField] private float _animsAttackComboSpeed = 1.6f;
        [SerializeField] private float _animsTransitionDuration = 0.1f;

        public void PlayAttackAnimationCombo(int comboStep, System.Action animDonePlayingCallback = null)
        {
            PlayerAnimState attackComboState = new("AttackCombo" + comboStep, 1f);
            print("attack name: " + attackComboState.ClipName);
            _animStateMachine.PlayAnimOnce(attackComboState, _animatorPrefix, _animatorSuffix, "AttackComboSpeed", _animsAttackComboSpeed, animDonePlayingCallback, _animsTransitionDuration);
        }

        // Animations used by player
        public class PlayerAnimState : EntityAnimState
        {
            public PlayerAnimState(string pClipName, float pAnimSpeed = 1f, int pNbVariations = -1)
                : base(pClipName, pAnimSpeed, pNbVariations)
            {

            }
        }
    }
}
