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

        public bool SetLoopedStateHealing()
        {
            PlayerAnimState healingState = PlayerAnimState.Healing;
            bool didAnimStateChange = _animStateMachine.GetCurrentLoopedState()?.ClipName != healingState.ClipName;
            _animStateMachine.SetLoopedState(healingState, _animatorPrefix, _animatorSuffix);

            return didAnimStateChange;
        }

        // Animations used by player
        public class PlayerAnimState : EntityAnimState
        {
            static public readonly PlayerAnimState Healing = new PlayerAnimState("Healing", 1f);
            public PlayerAnimState(string pClipName, float pAnimSpeed = 1f, int pNbVariations = -1)
                : base(pClipName, pAnimSpeed, pNbVariations)
            {

            }
        }
    }
}
