
using Bytes;
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public class AnimatedBossHealthBarUI : AnimatedHealthBarUI
    {
        [Header("AnimatedBossHealthBarUI ")]
        [SerializeField] private Image _imgPortrait;
        [SerializeField] private Image _imgTitle;
        [SerializeField] private GameObject _ultIndicator;
        // 0 miniboss, 1 boss
        [SerializeField] private Sprite[] _hpBarSprites;
        [SerializeField] private Sprite[] _portraitSprites;
        [SerializeField] private Sprite[] _titleSprites;


        protected override void Start()
        {
            base.Start();
            EventManager.AddEventListener(EventNames.UpdateBossHealthUI, HandleUpdateBossHealthUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateBossHealthUI, HandleUpdateBossHealthUI);
        }

        private void UpdateAllImages(bool isMiniBoss)
        {
            int index = isMiniBoss ? 0 : 1;
            _imgFilling.sprite = _hpBarSprites[index];
            _imgPortrait.sprite = _portraitSprites[index];
            _imgTitle.sprite = _titleSprites[index];
        }

        private void HandleUpdateBossHealthUI(BytesData data) 
        {
            var bossData = (data as UpdateBossHealthUIData);
            TakeDamage(bossData.HpFillAmount);

            // Set filling to zero instantly
            if (bossData.HpFillAmount <= 0)
            {
                _imgFillingWhite.fillAmount = 0;
                SetVisible(false);
            }
            else if(!GetIsVisible())
            {
                SetVisible(true);
                UpdateAllImages(bossData.IsMiniBoss);
                _ultIndicator.SetActive(bossData.IsShowUltIndicator);
                _imgFilling.color = (bossData.IsShowUltIndicator) ? Color.magenta : Color.white;
            }
        }
    }
}