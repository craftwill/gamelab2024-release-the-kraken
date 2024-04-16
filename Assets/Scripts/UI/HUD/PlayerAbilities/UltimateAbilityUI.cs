
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
        [SerializeField] private Image _imgUltIndicator;
        [SerializeField] private GameObject _ultIndicator;
        [Header("Control Sprites")]
        // Index 0 is pc and 1 is xbox controller
        [SerializeField] protected Sprite[] _controlSprites;
        // 0 dazzle is waiting, 1 razzle is waiting
        [SerializeField] protected Sprite[] _ultIndicatorSprites;

        private bool _isRazzle;

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
            EventManager.AddEventListener(EventNames.SetupHUD, HandleSetupHUD);
            EventManager.AddEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
            EventManager.AddEventListener(EventNames.UpdateUltimateUIIndicator, HandleUpdateUltimateUIIndicator);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.SetupHUD, HandleSetupHUD);
            EventManager.RemoveEventListener(EventNames.UpdateUltimateUI, HandleUpdateUltimateUI);
            EventManager.RemoveEventListener(EventNames.UpdateUltimateUIIndicator, HandleUpdateUltimateUIIndicator);
        }

        private void HandleSetupHUD(BytesData data)
        {
            _isRazzle = (data as SetupHUDData).IsRazzle;
            _imgUltIndicator.sprite = _ultIndicatorSprites[_isRazzle ? 0 : 1];
        }

        private void HandleUpdateUltimateUI(BytesData data)
        {
            var showUltCastMeAnim = (data as BoolDataBytes).BoolValue;

            _animator.SetBool("IsCastMe", showUltCastMeAnim);
        }

        private void HandleUpdateUltimateUIIndicator(BytesData data)
        {
            var showIndicator = (data as BoolDataBytes).BoolValue;

            _ultIndicator.SetActive(showIndicator);
        }
    }
}
