
using UnityEngine;

using Bytes;

namespace Kraken
{
    // Taken from soul shaper :eyes:
    public class FlickerVFX : MonoBehaviour
    {
        [SerializeField] private float _hitFlashMultiplier = 1f;
        [SerializeField] private Color _hitFlashColor = Color.red;
        private Renderer[] _renderers;
        private float _hitFlashIntensity = 1f;

        private Animate _currFlashAnim;

        public void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }

        public void PlayHurtFeedback()
        {
            //currently commented until it gets fixed
            //PlayHurtFeedbackWithFlashColor(_hitFlashColor);
        }

        public void PlayHurtFeedbackWithFlashColor(Color hitFlashColor)
        {
            if (_renderers.Length < 0) return;

            float usedHitFlashIntensity = _hitFlashIntensity * _hitFlashMultiplier;
            SetEmissionColorAlpha(hitFlashColor, usedHitFlashIntensity);

            _currFlashAnim?.Stop(false);
            _currFlashAnim = Animate.LerpSomething(Config.current.hitFlashDuration, (step) =>
            {
                SetEmissionColorAlpha(hitFlashColor, (1f - step) * usedHitFlashIntensity);
            }, () =>
            {
                SetEmissionColorAlpha(hitFlashColor, 0f);
            }, true);
        }

        private void SetEmissionColorAlpha(Color hitFlashColor, float alpha)
        {
            Color colInitial = Color.white;
            foreach (Renderer rend in _renderers)
            {
                foreach (Material mat in rend.materials)
                {
                    mat.SetColor("_Base_Color_Tint", Color.Lerp(colInitial, hitFlashColor, alpha));
                }
            }
        }
    }
}
