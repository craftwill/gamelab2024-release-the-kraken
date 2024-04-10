
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class PlayerWoolGaugeUI : AnimatedHealthBarUI
    {
        protected override void Start()
        {
            _imgFillingWhite.fillAmount = 0f;
            SetFillAmount(0f);
            EventManager.AddEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
        }

        private void HandleUpdateWoolQuantity(BytesData data)
        {
            int woolAmount = (data as IntDataBytes).IntValue;
            SetFillAmount((float) woolAmount / Config.current.maxWoolQuantity);
        }
    }
}
