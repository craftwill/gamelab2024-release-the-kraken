
using UnityEngine;

using Photon.Pun;

using Bytes;

namespace Kraken
{
    public class EntityCastAbilityComponent : MonoBehaviourPun
    {
        [SerializeField] protected float _cooldown;
        [SerializeField] private Animate _cooldownAnim;

        private Animate _canceledAnim;

        public virtual void CastAbility()
        {
            if (_cooldownAnim == null)
            {
                _cooldownAnim = Animate.Delay(_cooldown, () =>
                {
                    // Can Cast again
                    _cooldownAnim = null;
                }, timeScaled_: true);
            }
        }

        public bool CanCastAbility()
        {
            return _cooldownAnim == null;
        }

        protected void CancelCooldown() 
        {
            if (_canceledAnim != null) return;

            _cooldownAnim?.Stop(callEndFunction: false);
            // 0.1f second cancel delay in case player spams ability cancel
            _canceledAnim = Animate.Delay(0.1f, () =>
            {
                // Can Cast again
                _cooldownAnim = null;
                _canceledAnim = null;
            }, timeScaled_: true);
        }
    }
}