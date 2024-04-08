
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class AnimatedPlayerHealthBarUI : AnimatedHealthBarUI
    {
        protected override void Start()
        {
            base.Start();
            EventManager.AddEventListener(EventNames.UpdatePlayerHealthUI, HandleUpdateHealthBar);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdatePlayerHealthUI, HandleUpdateHealthBar);
        }

        private void HandleUpdateHealthBar(BytesData data)
        {
            float healthFillAmount = (data as FloatDataBytes).FloatValue;
            TakeDamage(healthFillAmount);
        }
    }
}