using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace Kraken.UI
{
    public class UltimateCooldownUI : KrakenUIElement
    {
        [SerializeField] private GameObject _txtAvailable;
        [SerializeField] private GameObject _txtCooldown;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
        }

        public void HandleUpdateUltimateUI(BytesData data)
        {
            bool available = ((BoolDataBytes)data).BoolValue;

            if (available)
            {
                _txtAvailable.SetActive(true);
                _txtCooldown.SetActive(false);
            }
            else
            {
                _txtAvailable.SetActive(false);
                _txtCooldown.SetActive(true);
            }
        }
    }
}