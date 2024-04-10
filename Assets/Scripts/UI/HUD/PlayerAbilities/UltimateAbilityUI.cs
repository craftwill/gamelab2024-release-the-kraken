
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public class UltimateAbilityUI : KrakenUIElement, IPlayerUIComponent
    {
        [Header("UltimateAbilityUI")]
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
    }
}
