using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public class HealthUI : KrakenUIElement
    {
        [SerializeField] Slider _slider;
        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdateHealthUI, UpdateHealthBar);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateHealthUI, UpdateHealthBar);
        }

        private void UpdateHealthBar(BytesData data)
        {
            float hp = ((FloatDataBytes)data).FloatValue;
            _slider.value = hp;
            Debug.Log("Update health " + hp);
        }
    }
}