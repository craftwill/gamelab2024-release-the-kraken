
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public abstract class AnimatedHealthBarUI : KrakenUIElement
    {
        [SerializeField] protected Image _imgFilling;
        [SerializeField] protected Image _imgFillingWhite;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected float _whiteFillSpeed = 1f;
        private float _targetFillAmount;

        protected virtual void Start()
        {
            _imgFillingWhite.fillAmount = 1f;
            SetFillAmount(1f);
        }

        private void Update()
        {
            _imgFillingWhite.fillAmount = Mathf.Lerp(_imgFillingWhite.fillAmount, _targetFillAmount, Time.deltaTime * _whiteFillSpeed);
        }

        protected virtual void TakeDamage(float fillAmount)
        {
            _animator?.Play("AnimatedHealthBarUI_takeDamage", -1, 0);
            SetFillAmount(fillAmount);
        }

        protected virtual void SetFillAmount(float fillAmount)
        {
            _targetFillAmount = fillAmount;
            _imgFilling.fillAmount = fillAmount;
        }
    }
}