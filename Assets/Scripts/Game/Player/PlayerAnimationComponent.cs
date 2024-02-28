using Kraken;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerAnimationComponent : EntityAnimationComponent
    {
        public void PlayAttackAnimationCombo(int comboStep)
        {
            PlayerAnimState attackComboState = new("AttackCombo" + comboStep, 1f);
            print("attack name: " + attackComboState.ClipName);
            _animStateMachine.PlayAnimOnce(attackComboState, _animatorPrefix, _animatorSuffix);
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
