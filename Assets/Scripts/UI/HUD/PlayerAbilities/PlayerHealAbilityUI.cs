
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class PlayerHealAbilityUI : PlayerUIComponent
    {
        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdatePlayerHealAbilityUI, HandleUpdatePlayerHealAbilityUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdatePlayerHealAbilityUI, HandleUpdatePlayerHealAbilityUI);
        }

        private void HandleUpdatePlayerHealAbilityUI(BytesData data) 
        {
            var isUsable = (data as BoolDataBytes).BoolValue;

            SetIsGreyedOut(!isUsable);
            _animator.SetBool("IsCastMe", isUsable);
        }
    }
}
