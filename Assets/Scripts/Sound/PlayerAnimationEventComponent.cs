using Kraken;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerAnimationEventComponent : MonoBehaviour
    {
        [SerializeField] private PlayerSoundComponent _soundComponent;
        public void FootstepEvent()
        {
            _soundComponent.PlayFootstep();
        }
    }
}
