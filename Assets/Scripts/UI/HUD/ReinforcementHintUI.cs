
using UnityEngine;

using Bytes;

using TMPro;

namespace Kraken.UI
{
    public class ReinforcementHintUI : KrakenUIElement
    {
        [SerializeField] private TextMeshProUGUI _txtReinforcementHint;
        [SerializeField] private Animator _reinforcementHintAnimator;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.ShowReinforcementHintUI, HandleShowReinforcementHintUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowReinforcementHintUI, HandleShowReinforcementHintUI);
        }

        private void HandleShowReinforcementHintUI(BytesData data)
        {
            string text = ((StringDataBytes)data).StringValue;
            _txtReinforcementHint.text = text;
            _reinforcementHintAnimator.Play("ReinforcementHintUI_appear", -1, 0);
        }
    }
}