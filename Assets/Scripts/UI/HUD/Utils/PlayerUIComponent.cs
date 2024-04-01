
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class PlayerUIComponent : KrakenUIElement
    {
        [Header("KrakenInputButtonUI")]
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
        protected int spriteIndexUsed;
        protected int controlSpriteIndexUsed;

        public void Init(bool isRazzle, bool isKeyboard)
        {
            spriteIndexUsed = isRazzle ? 0 : 1;
            controlSpriteIndexUsed = isKeyboard ? 0 : 1;
            _imgUsed.sprite = _iconSprites[spriteIndexUsed];
            _imgUsedBackground.sprite = _iconSprites[spriteIndexUsed];
            _imgControl.sprite = _controlSprites[controlSpriteIndexUsed];
        }

        public void SetIsGreyedOut(bool isGreyedOut) 
        {
            _isGreyedOut = isGreyedOut;
            if (_isGreyedOut)
            {
                _imgUsed.sprite = _greyedOutIconSprites[spriteIndexUsed];
                _imgUsedBackground.sprite = _greyedOutIconSprites[spriteIndexUsed];
            }
            else
            {
                _imgUsed.sprite = _iconSprites[spriteIndexUsed];
                _imgUsedBackground.sprite = _iconSprites[spriteIndexUsed];
            }
        }
    }
}
