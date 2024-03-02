using Bytes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace Kraken.UI
{
    public class UltimateCooldownUI : KrakenUIElement
    {
        [SerializeField] private GameObject _txtAvailable;
        [SerializeField] private GameObject _txtCooldown;
        private TextMeshProUGUI _cooldownText;
        private Coroutine _cooldownCoroutine;

        private void Start()
        {
            _cooldownText = _txtCooldown.GetComponent<TextMeshProUGUI>();
            EventManager.AddEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
        }

        public void HandleUpdateUltimateUI(BytesData data)
        {
            float cooldownTime = ((FloatDataBytes)data).FloatValue;
            if (cooldownTime == 0)
            {
                _txtAvailable.SetActive(true);
                _txtCooldown.SetActive(false);
            }
            else
            {
                if (_cooldownCoroutine == null)
                {
                    StartCoroutine(CooldownTimer(cooldownTime));
                    _txtAvailable.SetActive(false);
                    _txtCooldown.SetActive(true);
                }
            }
        }

        private IEnumerator CooldownTimer(float time)
        {
            int displayTime = (int)Mathf.Ceil(time);
            for (int i = displayTime; i >= 0; i--)
            {
                
                _cooldownText.text = i == 1 ? i.ToString() + " second before ultimate is recharged" : i.ToString() + " seconds before ultimate is recharged";
                yield return new WaitForSeconds(1);
            }
        }
    }
}