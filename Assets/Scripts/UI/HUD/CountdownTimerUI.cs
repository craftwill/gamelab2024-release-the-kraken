using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bytes;
using TMPro;
using Unity.VisualScripting;

namespace Kraken.UI
{
    public class CountdownTimerUI : KrakenUIElement
    {
        [Header("CountdownTimerUI")]
        [SerializeField] private Animator _countdownAnimator;
        [SerializeField] private TextMeshProUGUI _txtCountdown;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdateCountownTimerUI, HandleUpdateCountownTimerUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateCountownTimerUI, HandleUpdateCountownTimerUI);
        }

        public void HandleUpdateCountownTimerUI(BytesData data) 
        {
            UpdateCountownTimerUIData countdownData = (data as UpdateCountownTimerUIData);

            SetVisible(true);

            _txtCountdown.text = countdownData.CountdownTime.ToString();
            int currentCount = (int) countdownData.CountdownTime;
            Animate.Repeat(1f, () => 
            {
                // Keep doing countdown until last second
                if (currentCount == 1)
                {
                    SetVisible(false);
                    countdownData.EndCallback?.Invoke();
                    return false;
                }

                currentCount--;
                _txtCountdown.text = currentCount.ToString();
                _countdownAnimator.Play("AnimatorCountdownUI_step", -1, 0);

                return true;
            }, -1, true);
        }
    }
}
