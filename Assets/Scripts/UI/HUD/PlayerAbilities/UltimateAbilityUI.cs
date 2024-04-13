
using Bytes;
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public class UltimateAbilityUI : KrakenUIElement, IPlayerUIComponent
    {
        [Header("UltimateAbilityUI")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Image _imgControl;
        [Header("Control Sprites")]
        // Index 0 is pc and 1 is xbox controller
        [SerializeField] protected Sprite[] _controlSprites;

        // IPlayerUIComponent
        public void Init(bool isRazzle, bool isKeyboard)
        {
            _imgControl.sprite = _controlSprites[isKeyboard ? 0 : 1];
        }

        public void SetIsGreyedOut(bool isGreyedOut)
        {
            // No grey out for this component
        }
        // IPlayerUIComponent end

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
        }

        private void HandleUpdateUltimateUI(BytesData data)
        {
            var showUltCastMeAnim = (data as BoolDataBytes).BoolValue;

            _animator.SetBool("IsCastMe", showUltCastMeAnim);
        }
    }
}
