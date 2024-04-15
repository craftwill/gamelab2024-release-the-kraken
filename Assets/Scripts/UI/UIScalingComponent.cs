using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class UIScalingComponent : MonoBehaviour
    {
        private RectTransform _transform;
        private Vector3 _defaultValue;
        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            _defaultValue = _transform.localScale;
            _transform.localScale = _defaultValue * Config.current.uiScale;
            EventManager.AddEventListener(EventNames.UpdateUIScale, HandleUpdateUIScale);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateUIScale, HandleUpdateUIScale);
        }

        private void HandleUpdateUIScale(BytesData data)
        {
            _transform.localScale = _defaultValue * Config.current.uiScale;
        }
    }
}
