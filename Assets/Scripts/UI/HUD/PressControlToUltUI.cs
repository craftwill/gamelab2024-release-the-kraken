
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class PressControlToUltUI : KrakenUIElement, IPlayerUIComponent
    {
        [SerializeField] private Image _imgControl;
        // 0 keyboard, 1 controller
        [SerializeField] private Sprite[] _controlSprites;

        // IPlayerUIComponent
        public void Init(bool isRazzle, bool isKeyboard)
        {
            _imgControl.sprite = _controlSprites[isRazzle ? 0 : 1];
        }

        public void SetIsGreyedOut(bool isGreyedOut)
        {
            // No grey out
        }
        // end IPlayerUIComponent

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdatePressControlToUltUI, HandleUpdatePressControlToUltUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdatePressControlToUltUI, HandleUpdatePressControlToUltUI);
        }

        private void HandleUpdatePressControlToUltUI(BytesData data)
        {
            var isShow = (data as BoolDataBytes).BoolValue;

            SetVisible(isShow);
        }
    }
}
