
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class AbilityUI : KrakenUIElement
    {
        [SerializeField] private Image _imgFilling;
        [SerializeField] private Animator _abilityAnimator;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.StartAbilityCooldown, HandleStartAbilityCooldown);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.StartAbilityCooldown, HandleStartAbilityCooldown);
        }

        private void HandleStartAbilityCooldown(BytesData data)
        {
            float cooldownDuration = ((FloatDataBytes)data).FloatValue;

            Animate.LerpSomething(cooldownDuration , (float step) => 
            {
                _imgFilling.fillAmount = step;
            }, () =>
            {
                _imgFilling.fillAmount = 1f;
                _abilityAnimator.Play("AbilityUI_cooldownDone");
                // Play sound cuz ability is ready here...

            }, timeScaled_: true);
        }
    }
}
