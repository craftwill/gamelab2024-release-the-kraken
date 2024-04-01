
using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public class UltimateAbilityUI : KrakenUIElement
    {
        [SerializeField] private Image _imgWoolGauge;

        private void Start()
        {
            SetWoolGaugeCount(Config.current.initialWoolQuantity);
            EventManager.AddEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
        }

        private void HandleUpdateWoolQuantity(BytesData data)
        {
            int woolAmount = (data as IntDataBytes).IntValue;
            SetWoolGaugeCount(woolAmount);
        }

        private void SetWoolGaugeCount(int woolAmount) 
        {
            float fillAmount = (float)woolAmount / Config.current.maxWoolQuantity;
            _imgWoolGauge.fillAmount = fillAmount;
        }
    }
}
