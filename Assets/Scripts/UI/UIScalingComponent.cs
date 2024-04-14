using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class UIScalingComponent : MonoBehaviour
    {
        private RectTransform _transform;
        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            _transform.localScale = Vector3.one * Config.current.uiScale;
            EventManager.AddEventListener(EventNames.UpdateUIScale, HandleUpdateUIScale);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateUIScale, HandleUpdateUIScale);
        }

        private void HandleUpdateUIScale(BytesData data)
        {
            _transform.localScale = Vector3.one * Config.current.uiScale;
        }
    }
}
