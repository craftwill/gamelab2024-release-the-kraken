using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class BossAnimationComponent : EntityAnimationComponent
    {
        public void PlayStarfallAttack(System.Action animDonePlayingCallback = null)
        {
            BossAnimState starfall = BossAnimState.Starfall;
            _animStateMachine.PlayAnimOnce(starfall, _animatorPrefix, _animatorSuffix, "", 1f, animDonePlayingCallback);
        }

        public void PlayRingsOfLightAttack(System.Action animDonePlayingCallback = null)
        {
            BossAnimState rol = BossAnimState.RingsOfLight;
            _animStateMachine.PlayAnimOnce(rol, _animatorPrefix, _animatorSuffix, "", 1f, animDonePlayingCallback);
        }

        // Animations used by the boss
        public class BossAnimState : EntityAnimState
        {
            static public readonly BossAnimState Starfall = new BossAnimState("Attack01_StarFall", 1f);
            static public readonly BossAnimState RingsOfLight = new BossAnimState("Attack02_RingsOfLight", 1f);
            public BossAnimState(string pClipName, float pAnimSpeed = 1f, int pNbVariations = -1)
                : base(pClipName, pAnimSpeed, pNbVariations)
            {

            }
        }
    }
}