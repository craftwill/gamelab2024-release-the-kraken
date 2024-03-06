
using UnityEngine;

using Photon.Pun;

using Bytes;

namespace Kraken
{
    public class EntityCastAbilityComponent : MonoBehaviourPun
    {
        [SerializeField] protected float _cooldown;
        [SerializeField] private Animate _cooldownAnim;

        public virtual void CastAbility()
        {
            if (_cooldownAnim == null)
            {
                _cooldownAnim = Animate.Delay(_cooldown, () =>
                {
                    // Can Cast again
                    _cooldownAnim = null;
                });
            }
        }

        public bool CanCastAbility()
        {
            return _cooldownAnim == null;
        }
    }
}