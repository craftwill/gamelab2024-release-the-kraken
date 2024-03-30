
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class AnimatedPlayerHealthBarUI : AnimatedHealthBarUI
    {
        [Header("PlayerHealthBar")]
        [SerializeField] private Image _imgPortrait;
        [SerializeField] private Sprite _regularSprite;
        [SerializeField] private Sprite _hurtSprite;

        private Animate _animSpriteHurt;

        protected override void Start()
        {
            base.Start();
            EventManager.AddEventListener(EventNames.UpdatePlayerHealthUI, HandleUpdateHealthBar);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdatePlayerHealthUI, HandleUpdateHealthBar);
        }

        private void HandleUpdateHealthBar(BytesData data)
        {
            float healthFillAmount = (data as FloatDataBytes).FloatValue;
            TakeDamage(healthFillAmount);

            _imgPortrait.sprite = _hurtSprite;
            _animSpriteHurt?.Stop(callEndFunction: false);
            _animSpriteHurt = Animate.Delay(0.25f, () => 
            {
                _imgPortrait.sprite = _regularSprite;
            });
        }
    }
}