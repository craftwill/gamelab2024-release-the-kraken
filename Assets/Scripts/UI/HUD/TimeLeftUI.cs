
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class TimeLeftUI : KrakenUIElement
    {
        [SerializeField] private Image _imgProgress;
        [SerializeField] private Animator _imgProgressAnimator;
        [SerializeField] private Vector3 _localStartPos;
        [SerializeField] private Vector3 _localEndPos;
        private Animate _timeLeftAnim;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.InitTimeLeftUI, HandleUpdateTimeLeftUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.InitTimeLeftUI, HandleUpdateTimeLeftUI);
        }

        private void Update()
        {
            float progress = _timeLeftAnim.GetTimeLeft() / _timeLeftAnim.GetDuration();
            _imgProgress.transform.localPosition = Vector3.Lerp(_localStartPos, _localEndPos, progress);

            if (progress > 0.8f) 
            {
                _imgProgressAnimator.SetTrigger("IsDangerZone");
            }
        }

        private void HandleUpdateTimeLeftUI(BytesData data) 
        {
            _timeLeftAnim = (data as ObjectDataBytes).ObjectValue as Animate;
        }
    }
}
