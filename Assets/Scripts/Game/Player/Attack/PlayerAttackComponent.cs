using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using System.Collections;
using UnityEngine.InputSystem;
using Bytes;

namespace Kraken
{
    public class PlayerAttackComponent : MonoBehaviourPun
    {
        private bool _isOwner;
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private List<AttackSO> _attacks = new List<AttackSO>();
        [SerializeField] private InputActionReference _attackInput;
        [SerializeField] private PauseManager _pauseManager = null;

        private bool _isFreeToAttack = true;
        private AttackSO _currentAttack;
        private bool _controlsEnabled = true;
        private bool _ultimateRunning = false;
        private bool _inProgress;
        private List<GameObject> _colliders = new List<GameObject>();

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                _attackInput.action.performed += AttackPressed;
                _currentAttack = _attacks[0];
                _attacks.ForEach(x =>
                {
                    var c = Instantiate(x.colliderGameObject, transform);
                    c.transform.position += transform.forward;
                    var idc = c.GetComponent<InflictDamageComponent>();
                    idc.Damage = x.damage;
                    idc.Source = transform;
                    _colliders.Add(c);
                });
                if (_pauseManager == null)
            {
                _pauseManager = Object.FindObjectOfType<PauseManager>(); //temp but i don't wanna lock the game scene
            }
            }
            EventManager.AddEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
        }

        private void OnDestroy()
        {
            if (_isOwner)
            {
                _attackInput.action.performed -= AttackPressed;
            }
            EventManager.RemoveEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
        }

        public void SetAttacksControlsEnabled(bool controlsEnabled)
        {
            if (photonView.AmOwner)
            {
                _controlsEnabled = controlsEnabled;
            }
        }

        private void AttackPressed(InputAction.CallbackContext callback)
        {
            if (!_controlsEnabled || _pauseManager.Paused || _ultimateRunning) return;

            if (_isFreeToAttack)
            {
                if (_inProgress)
                {
                    //if the animation of the current attack has not ended, perform the next attack(or the first one if this is the last one)
                    _currentAttack = _currentAttack?.nextAttack ?? _attacks[0];
                }
                else
                {
                    _currentAttack = _attacks[0];
                }
                PerformAttack(_currentAttack);
            }
        }

        private void PerformAttack(AttackSO attack)
        {
            StopCoroutine(nameof(InProgressFunc));
            StopCoroutine(nameof(AnimDoneBufferTimer));
            _isFreeToAttack = false;
            _inProgress = true;
            StartCoroutine(InProgressFunc(attack));
            StartCoroutine(AttackFunc(attack));
        }

        private IEnumerator AttackFunc(AttackSO attack)
        {
            void AnimDonePlayingCallback()
            {
                if (_isFreeToAttack)
                {
                    StartCoroutine(AnimDoneBufferTimer(attack));
                }
            }

            _playerEntity.PlayAttackAnimationCombo(attack.comboStep, AnimDonePlayingCallback);

            yield return new WaitForSeconds(attack.timeBeforeHitboxDuration);
            _colliders[attack.comboStep - 1].SetActive(true);
            EventManager.Dispatch(EventNames.PlayerAttackStart, new FloatDataBytes(Config.current.attackMoveSpeed));
            yield return new WaitForSeconds(attack.hitboxDuration);
            _colliders[attack.comboStep - 1].SetActive(false);
            EventManager.Dispatch(EventNames.PlayerAttackEnd, null);
        }

        private IEnumerator InProgressFunc(AttackSO attack)
        {
            yield return new WaitForSeconds(attack.attackLockLength);
            _isFreeToAttack = true;
        }

        private IEnumerator AnimDoneBufferTimer(AttackSO attack)
        {
            yield return new WaitForSeconds(attack.animDoneBufferTimer);
            _inProgress = false;
        }

        private void HandleUltimateRunning(BytesData data)
        {
            _ultimateRunning = ((BoolDataBytes)data).BoolValue;
        }
    }
}