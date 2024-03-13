using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using System.Collections;
using UnityEngine.InputSystem;

namespace Kraken
{
    public class PlayerAttackComponent : MonoBehaviourPun
    {
        private bool _isOwner;
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private List<AttackSO> _attacks = new List<AttackSO>();
        [SerializeField] private InputActionReference _attackInput;
        [SerializeField] public bool IsFreeToAttack { get; set; } = true;

        private AttackSO _currentAttack;
        private bool _controlsEnabled = true;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                _attackInput.action.performed += PerformAttack;
                _currentAttack = _attacks[0];
            }
        }

        private void OnDestroy()
        {
            if (_isOwner)
            {
                _attackInput.action.performed -= PerformAttack;
            }
        }

        public void SetAttacksControlsEnabled(bool controlsEnabled)
        {
            if (photonView.AmOwner)
            {
                _controlsEnabled = controlsEnabled;
            }
        }


        private void PerformAttack(InputAction.CallbackContext callback)
        {
            if (!_controlsEnabled) return;

            if (IsFreeToAttack)
            {
                if (IsInProgress())
                {
                    if (nextAttack is null)
                    {
                        // Input buffer here?
                        // _inProgress = false;
                        // this.PerformAttack(handle, callback);
                    }
                    else
                    {
                        nextAttack.PerformAttack(handle, callback);
                    }
                    return;
                }
                handle.IsFreeToAttack = false;

                _inProgress = true;
                handle.StartCoroutine(InProgressFunc(handle));

                handle.StartCoroutine(AttackFunc(handle));
            }

        }

        /*private IEnumerator AttackFunc(PlayerAttackComponent handle)
        {
            void AnimDonePlayingCallback()
            {
                handle.IsFreeToAttack = true;
            }

            _playerEntity.PlayAttackAnimationCombo(comboStep, AnimDonePlayingCallback);

            yield return new WaitForSeconds(timeBeforeHitboxDuration);
            _collider.SetActive(true);
            EventManager.Dispatch(EventNames.PlayerAttackStart, new FloatDataBytes(Config.current.attackMoveSpeed));
            yield return new WaitForSeconds(hitboxDuration);
            _collider.SetActive(false);
            EventManager.Dispatch(EventNames.PlayerAttackEnd, null);
        }

        private IEnumerator InProgressFunc(PlayerAttackComponent handle)
        {
            yield return new WaitForSeconds(totalAttackLength);
            _inProgress = false;
        }
        */
        public bool IsInProgress()
        {
            return _inProgress ||
                (nextAttack is null ? false : nextAttack.IsInProgress());
        }
    }
}