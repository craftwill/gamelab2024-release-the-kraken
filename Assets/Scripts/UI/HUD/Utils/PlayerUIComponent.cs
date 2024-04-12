
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public interface IPlayerUIComponent
    {
        void Init(bool isRazzle, bool isKeyboard);
        void SetIsGreyedOut(bool isGreyedOut);
    }
    public class PlayerUIComponent : KrakenUIElement, IPlayerUIComponent
    {
        [Header("PlayerUIComponent")]
        [SerializeField] protected Image _imgUsed;
        [SerializeField] protected Image _imgUsedBackground;
        [SerializeField] protected Image _imgControl;
        [SerializeField] protected Animator _animator;

        [Header("Button Sprites")]
        // Index 0 is razzle and 1 is dazzle and 2
        [SerializeField] protected Sprite[] _iconSprites;
        [SerializeField] protected Sprite[] _greyedOutIconSprites;

        [Header("Control Sprites")]
        // Index 0 is pc and 1 is xbox controller
        [SerializeField] protected Sprite[] _controlSprites;

        protected bool _isGreyedOut;
        protected int _spriteIndexUsed;
        protected int _controlSpriteIndexUsed;

        public virtual void Init(bool isRazzle, bool isKeyboard)
        {
            _spriteIndexUsed = isRazzle ? 0 : 1;
            _controlSpriteIndexUsed = isKeyboard ? 1 : 0;
            _imgUsed.sprite = _iconSprites[_spriteIndexUsed];
            _imgUsedBackground.sprite = _iconSprites[_spriteIndexUsed];
            _imgControl.sprite = _controlSprites[_controlSpriteIndexUsed];
        }

        public void SetIsGreyedOut(bool isGreyedOut) 
        {
            _isGreyedOut = isGreyedOut;
            _imgControl.gameObject.SetActive(!_isGreyedOut);
            if (_isGreyedOut)
            {
                _imgUsed.sprite = _greyedOutIconSprites[_spriteIndexUsed];
                _imgUsedBackground.sprite = _greyedOutIconSprites[_spriteIndexUsed];
            }
            else
            {
                _imgUsed.sprite = _iconSprites[_spriteIndexUsed];
                _imgUsedBackground.sprite = _iconSprites[_spriteIndexUsed];
            }
        }
    }
}
