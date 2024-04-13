
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class PlayerTotemAbilityUI : PlayerUIComponent
    {
        public override void Init(bool isRazzle, bool isKeyboard)
        {
            base.Init(isRazzle, isKeyboard);

            SetIsGreyedOut(true);
        }

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdatePlayerTotemAbilityUI, HandleUpdatePlayerTotemAbilityUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdatePlayerTotemAbilityUI, HandleUpdatePlayerTotemAbilityUI);
        }

        private void HandleUpdatePlayerTotemAbilityUI(BytesData data)
        {
            var isInRangeToheal = (data as BoolDataBytes).BoolValue;

            SetIsGreyedOut(!isInRangeToheal);
            _animator.SetBool("IsCastMe", isInRangeToheal);
        }
    }
}
