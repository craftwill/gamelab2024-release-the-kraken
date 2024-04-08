
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    // Manages the player profile state according to player type and if the ultimate is active.
    public class PlayerProfileUI : KrakenUIElement
    {
        private const float HURT_SPRITE_CHANGE_DELAY = 0.28f;

        [Header("PlayerProfileUI")]
        [SerializeField] private Animator _animator;
        [SerializeField] private OtherPlayerHealthUI _otherPlayerHealthUI;
        [SerializeField] private Image _imgBackground;
        [SerializeField] private Image _imgPortrait;
        [SerializeField] private Image _imgPortraitTail;

        [Header("Sprites")]
        // 0 razzle, 1 dazzle
        [SerializeField] private Sprite[] _backgroundSprites;
        // 0-1-2 is razzle normal-hurt-rainbow
        // 3-4-5 is dazzle normal-hurt-rainbow
        [SerializeField] private Sprite[] _portraitSprites;
        [SerializeField] private Sprite[] _portraitTailSprites;

        private bool _isRazzle;
        private bool _isRainbow;

        private Animate _hurtAnim;

        // Player profile can be normal
        // Player profile can be rainbow + animated during ultimate
        // Player profile

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
            EventManager.AddEventListener(EventNames.UpdatePlayerHealthUI, HandleUpdateHealthBar);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
            EventManager.RemoveEventListener(EventNames.UpdatePlayerHealthUI, HandleUpdateHealthBar);
        }

        public void Init(bool isRazzle)
        {
            _isRazzle = isRazzle;
            SetNormalState();
        }

        private void HandleUltimateRunning(BytesData data)
        {
            bool isRunning = ((BoolDataBytes)data).BoolValue;

            _isRainbow = isRunning;
            if (_hurtAnim == null)
            {
                UpdateAllSprites(_isRainbow ? 2 : 0);
            }
        }

        private void HandleUpdateHealthBar(BytesData data)
        {
            PlayHurtAnim();
        }

        public void SetNormalState()
        {
            _isRainbow = false;
            UpdateAllSprites(0);
        }

        private void PlayHurtAnim()
        {
            _animator.Play("PlayerProfileUI_hurt", -1, 0);

            UpdateAllSprites(1);
            _hurtAnim?.Stop(callEndFunction: false);
            _hurtAnim = Animate.LerpSomething(HURT_SPRITE_CHANGE_DELAY, (float step) =>
            {
                _imgBackground.color = Color.Lerp(Color.red, Color.white, step);
                _imgPortrait.color = Color.Lerp(Color.red, Color.white, step);
                _imgPortraitTail.color = Color.Lerp(Color.red, Color.white, step);
            }, () => 
            {
                UpdateAllSprites(_isRainbow ? 2 : 0);
                _hurtAnim = null;
            }, timeScaled_: false);
        }

        public void SetRainbowState()
        {
            _isRainbow = true;
            UpdateAllSprites(2);
        }

        private void UpdateAllSprites(int offset = 0)
        {
            int indexOffset = (_isRazzle ? 0 : 3) + offset;

            _imgBackground.sprite = _backgroundSprites[(_isRazzle ? 0 : 1)];
            _imgPortrait.sprite = _portraitSprites[indexOffset];
            _imgPortraitTail.sprite = _portraitTailSprites[indexOffset];
            _otherPlayerHealthUI.SetIsRainbow(_isRainbow);
        }
    }
}
