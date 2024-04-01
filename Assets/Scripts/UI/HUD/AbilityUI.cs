
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class AbilityUI : PlayerUIComponent
    {
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
                _imgUsed.fillAmount = step;
            }, () =>
            {
                _imgUsed.fillAmount = 1f;
                _animator.Play("AbilityUI_cooldownDone");
                // Play sound cuz ability is ready here...

            }, timeScaled_: true);
        }
    }
}
