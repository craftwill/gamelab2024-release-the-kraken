using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bytes;

namespace Kraken
{
    public class AttackComboVfxProgress : MonoBehaviour
    {
        [SerializeField] private Renderer[] _vfxRenderers;
        [SerializeField] private float _progressDuration = 0.35f;
        private void OnEnable()
        {
            Animate.LerpSomething(_progressDuration, (float step) => 
            {
                foreach (var renderer in _vfxRenderers)
                {
                    renderer.material.SetFloat("_Progress", step);
                }
            }, () => 
            {
                foreach (var renderer in _vfxRenderers)
                {
                    renderer.material.SetFloat("_Progress", 1f);
                }
            }, timeScaled_: true);
        }
    }
}
