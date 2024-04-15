
using UnityEngine;

namespace Kraken.UI
{
    public class KrakenUIElement : MonoBehaviour
    {
        [Header("KrakenUIElement - Settings")]
        [SerializeField] private bool _startsVisible = true;
        [SerializeField] private bool _interactableWhenVisible = true;
        [SerializeField] private float _visibleAlpha = 1f;

        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            SetVisible(_startsVisible);
        }

        public virtual void SetVisible(bool isVisible)
        {
            _canvasGroup.alpha = isVisible ? _visibleAlpha : 0;
            _canvasGroup.interactable = _interactableWhenVisible && isVisible;
            _canvasGroup.blocksRaycasts = _interactableWhenVisible && isVisible;
        }
    }
}
