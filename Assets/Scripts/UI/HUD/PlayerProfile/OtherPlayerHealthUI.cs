using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class OtherPlayerHealthUI : KrakenUIElement
    {
        private const float HURT_SPRITE_CHANGE_DELAY = 0.28f;

        [SerializeField] private Image _imgHpFilling;
        [SerializeField] private Image _imgPortrait;

        [SerializeField] private Sprite[] _portraitSprites;

        private bool _isRainbow;

        private Animate _hurtAnim;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdateOtherPlayerHealthUI, HandleUpdateHealthBar);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateOtherPlayerHealthUI, HandleUpdateHealthBar);
        }

        private void HandleUpdateHealthBar(BytesData data)
        {
            float hp = ((FloatDataBytes)data).FloatValue;
            _imgHpFilling.fillAmount = hp;
            PlayHurtAnim();
        }

        private void PlayHurtAnim() 
        {
            _imgPortrait.sprite = _portraitSprites[1];
            _hurtAnim?.Stop(callEndFunction: false);
            _hurtAnim = Animate.Delay(HURT_SPRITE_CHANGE_DELAY, () => 
            {
                _imgPortrait.sprite = _portraitSprites[(_isRainbow ? 2 : 0)];
                _hurtAnim = null;
            });
        }

        public void SetIsRainbow(bool isRainbow)
        {
            _isRainbow = isRainbow;

            if (_hurtAnim == null)
            {
                _imgPortrait.sprite = _portraitSprites[(isRainbow ? 2 : 0)];
            }
        }
    }
}