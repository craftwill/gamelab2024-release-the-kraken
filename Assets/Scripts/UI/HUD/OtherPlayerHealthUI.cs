using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class OtherPlayerHealthUI : KrakenUIElement
    {
        [SerializeField] private Image imgHpFilling;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdateOtherPlayerHealthUI, HandleUpdateHealthBar);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateOtherPlayerHealthUI, HandleUpdateHealthBar);
        }

        private void HandleUpdateHealthBar(BytesData data)
        {
            float hp = ((FloatDataBytes)data).FloatValue;
            imgHpFilling.fillAmount = hp;
        }
    }
}