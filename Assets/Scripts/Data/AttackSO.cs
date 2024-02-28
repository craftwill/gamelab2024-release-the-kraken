using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using System;
using Bytes;

namespace Kraken
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Kraken/Create Attack")]
    public class AttackSO : ScriptableObject
    {
        [Tooltip("Name of the attack, currently only for debugging purposes")]
        public string attackName;
        public InputActionReference attackInput;
        [Tooltip("For combo purposes, the attack will be called by the next attack parameter of another attack. If false, the attackInput is useless as it uses the previous attack's input")]
        public bool canBeActivatedFromNeutral;
        [Tooltip("Next attack if you combo")]
        public AttackSO nextAttack;
        [Tooltip("The hitbox of the attack, insert a gameobject with a collider component and the \"DealDamage\" tag, see Assets/Prefab/Collider for an example")]
        public GameObject colliderGameObject;
        [Range(0.0f, 1.0f), Tooltip("How long until the hitbox comes out")]
        public float timeBeforeHitboxDuration;
        [Range(0.0f, 1.0f), Tooltip("How long does the hitbox stays out")]
        public float hitboxDuration;
        [Range(0.0f, 10.0f), Tooltip("How long until the animation can be cancelled for a new attack")]
        public float attackCancelDuration;
        [Tooltip("How long the animation is")]
        public float totalAttackLength;
        public int damage;
        public int comboStep = 1;

        private GameObject _collider;
        private Action<InputAction.CallbackContext> _inputCallBackHandler;
        private bool _inProgress = false;

        private PlayerEntity _playerEntity;

        public void Subscribe(PlayerAttackComponent handle, PlayerEntity playerEntity)
        {
            _playerEntity = playerEntity;

            _inputCallBackHandler = (context) => PerformAttack(handle, context);
            if (canBeActivatedFromNeutral)
                attackInput.action.performed += _inputCallBackHandler;

            _collider = Instantiate(colliderGameObject, handle.transform);
            _collider.transform.position += handle.transform.forward;
            var idc = _collider.GetComponent<InflictDamageComponent>();
            if (idc is not null) idc.Damage = damage;
            else Debug.LogWarning("Warning: the collider doesn't have an InflictDamageComponent attached");

            _inProgress = false;
        }

        public void Unsubscribe()
        {
            if(canBeActivatedFromNeutral)
                attackInput.action.performed -= _inputCallBackHandler;
        }
        
        private void PerformAttack(PlayerAttackComponent handle, InputAction.CallbackContext callback)
        {
            if (handle.IsFreeToAttack)
            {
                if (IsInProgress())
                {
                    if (nextAttack is null)
                    {
                        _inProgress = false;
                        this.PerformAttack(handle, callback);
                    }
                    else nextAttack.PerformAttack(handle, callback);
                    return;
                }
                handle.IsFreeToAttack = false;
                handle.StartCoroutine(FreeToAttackFunc(handle));

                _inProgress = true;
                handle.StartCoroutine(InProgressFunc(handle));

                handle.StartCoroutine(AttackFunc(handle));
            }

        }

        private IEnumerator AttackFunc(PlayerAttackComponent handle)
        {
            _playerEntity.PlayAttackAnimationCombo(comboStep);

            yield return new WaitForSeconds(timeBeforeHitboxDuration);
            _collider.SetActive(true);
            EventManager.Dispatch(EventNames.PlayerAttackStart, new FloatDataBytes(Config.current.attackMoveSpeed));
            yield return new WaitForSeconds(hitboxDuration);
            _collider.SetActive(false);
            EventManager.Dispatch(EventNames.PlayerAttackEnd, null);
        }

        private IEnumerator FreeToAttackFunc(PlayerAttackComponent handle)
        {
            yield return new WaitForSeconds(attackCancelDuration);
            handle.IsFreeToAttack = true;
        }

        private IEnumerator InProgressFunc(PlayerAttackComponent handle)
        {
            yield return new WaitForSeconds(totalAttackLength);
            _inProgress = false;
        }

        public bool IsInProgress()
        {
            return _inProgress || 
                (nextAttack is null ? false : nextAttack.IsInProgress());
        }
    }
}