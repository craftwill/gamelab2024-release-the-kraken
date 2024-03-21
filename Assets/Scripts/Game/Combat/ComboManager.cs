using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class ComboManager : MonoBehaviour
    {
        private float _pitch = 0f;
        private Coroutine _pitchCoroutine = null;
        private string _comboPitchKey = Config.COMBO_PITCH;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.IncreaseCombo, HandleIncreaseCombo);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.IncreaseCombo, HandleIncreaseCombo);
        }

        private void HandleIncreaseCombo(BytesData data)
        {
            if (_pitchCoroutine != null)
            {
                StopCoroutine(_pitchCoroutine);
                _pitchCoroutine = null;
            }
            _pitchCoroutine = StartCoroutine(PitchCoroutine());
        }

        private IEnumerator PitchCoroutine()
        {
            _pitch += Config.current.comboPitchIncrement;
            AkSoundEngine.SetRTPCValue(_comboPitchKey, _pitch);
            yield return new WaitForSeconds(Config.current.comboMaxHitInterval);
            _pitch = 0;
            AkSoundEngine.SetRTPCValue(_comboPitchKey, _pitch);
        }
    }
}
