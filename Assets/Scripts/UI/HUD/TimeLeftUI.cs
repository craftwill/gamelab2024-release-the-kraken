
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class TimeLeftUI : KrakenUIElement
    {
        [Header("TimeLeftUI")]
        [SerializeField] private Animator _timeLeftAnimator;
        [Header("Progress")]
        [SerializeField] private GameObject _progressRoot;
        [SerializeField] private Image _imgProgress;
        [SerializeField] private Animator _progressAnimator;
        [SerializeField] private Vector3 _localStartPos;
        [SerializeField] private Vector3 _localEndPos;
        private Animate _timeLeftAnim;
        private bool _isDangerZone = false;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.InitTimeLeftUI, HandleUpdateTimeLeftUI);
            EventManager.AddEventListener(EventNames.ShowDefeatTimeLeftUI, HandleShowDefeatTimeLeftUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.InitTimeLeftUI, HandleUpdateTimeLeftUI);
            EventManager.RemoveEventListener(EventNames.ShowDefeatTimeLeftUI, HandleShowDefeatTimeLeftUI);
        }

        private void Update()
        {
            if (_timeLeftAnim == null || _timeLeftAnim.GetIsDone())
            {
                return;
            }

            float progress = 1 - (_timeLeftAnim.GetTimeLeft() / _timeLeftAnim.GetDuration());
            _progressRoot.transform.localPosition = Vector3.Lerp(_localStartPos, _localEndPos, progress);

            if (!_isDangerZone && progress > Config.current.timeLeftUIProgressShowWarningTreshold)
            {
                _isDangerZone = true;
                _progressAnimator.SetTrigger("IsDangerZone");
                _timeLeftAnimator.SetTrigger("IsDangerZone");
            }
        }

        private void HandleUpdateTimeLeftUI(BytesData data) 
        {
            _timeLeftAnim = (data as ObjectDataBytes).ObjectValue as Animate;
        }

        private void HandleShowDefeatTimeLeftUI(BytesData data)
        {
            _timeLeftAnimator.SetTrigger("IsDefeated");
        }
    }
}
