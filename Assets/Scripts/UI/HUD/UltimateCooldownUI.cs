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
        private TextMeshProUGUI _text;
        private TextMeshProUGUI _cooldownText;
        private Coroutine _cooldownCoroutine;
        private int _woolQuantity = 0;
        private bool _inUltimate = false;

        private void Start()
        {
            _text = _txtAvailable.GetComponent<TextMeshProUGUI>();
            _cooldownText = _txtCooldown.GetComponent<TextMeshProUGUI>();
            //EventManager.AddEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
            EventManager.AddEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
            EventManager.AddEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
            _text.text = "0 wool\nNeed " + Config.current.ultimateMinWool + " to use ultimate";
        }

        private void OnDestroy()
        {
            //EventManager.RemoveEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
            EventManager.RemoveEventListener(EventNames.UpdateWoolQuantity, HandleUpdateWoolQuantity);
            EventManager.RemoveEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
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

        private void HandleUpdateWoolQuantity(BytesData data)
        {
            _woolQuantity = ((IntDataBytes)data).IntValue;
            RefreshText();
        }

        private void HandleUltimateRunning(BytesData data)
        {
            _inUltimate = ((BoolDataBytes)data).BoolValue;
            RefreshText();
        }

        private void RefreshText()
        {
            string text = _woolQuantity.ToString() + " wool";
            if (_inUltimate)
            {
                text += "\n ";
            }
            else if (_woolQuantity < Config.current.ultimateMinWool)
            {
                text += "\nNeed " + Config.current.ultimateMinWool + " to use ultimate";
            }
            else
            {
                text += "\nSpacebar/LT+RT to use ultimate";
            }
            _text.text = text;
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