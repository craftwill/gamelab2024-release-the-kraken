using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public class EntityAnimationComponent : MonoBehaviourPun
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _animatorSuffix;

        private int _hurtStep = 1;

        private void Start()
        {
            if (_animator == null) return;

            _animator.speed = Random.Range(0.9f, 1.1f);
        }

        public void PlayHurtAnim()
        {
            if (_animator == null) return;

            _animator.Play(_animatorSuffix + "_hurt" + _hurtStep);
            _hurtStep++;
            if (_hurtStep > 2)
            {
                _hurtStep = 1;
            }
        }
    }
}
