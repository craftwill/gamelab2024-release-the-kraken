
using Bytes;

namespace Kraken.UI
{
    public class AnimatedBossHealthBarUI : AnimatedHealthBarUI
    {
        protected override void Start()
        {
            base.Start();
            EventManager.AddEventListener(EventNames.UpdateBossHealthUI, HandleUpdateBossHealthUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateBossHealthUI, HandleUpdateBossHealthUI);
        }

        private void HandleUpdateBossHealthUI(BytesData data) 
        {
            float healthFillAmount = (data as FloatDataBytes).FloatValue;
            TakeDamage(healthFillAmount);

            // Set filling to zero instantly
            if (healthFillAmount <= 0)
            {
                _imgFillingWhite.fillAmount = 0;
            }
        }
    }
}