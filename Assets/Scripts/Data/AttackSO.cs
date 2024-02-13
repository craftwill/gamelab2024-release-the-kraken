using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using System;

namespace Kraken
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Kraken/Create Attack")]
    public class AttackSO : ScriptableObject
    {
        public InputActionReference attackInput;
        public GameObject colliderGameObject;
        [Range(0.0f, 1.0f), Tooltip("How long until the hitbox comes out")]
        public float timeBeforeHitbox;
        [Range(0.0f, 1.0f), Tooltip("How long does the hitbox stays out")]
        public float hitboxDuration;
        [Range(0.0f, 10.0f), Tooltip("How long until the animation can be cancelled for a new attack, not implemented yet")]
        public float attackCancel;
        public int damage;
        

        private GameObject _collider;
        private Action<InputAction.CallbackContext> _inputCallBackHandler;
        private IEnumerator _attackCoroutine;
        
        public void Subscribe(MonoBehaviourPun handle)
        {
            _inputCallBackHandler = (context) => PerformAttack(handle, context);
            attackInput.action.performed += _inputCallBackHandler;
            _collider = Instantiate(colliderGameObject, handle.transform);
            _collider.transform.position += handle.transform.forward;            
        }
        public void Unsubscribe()
        {
            attackInput.action.performed -= _inputCallBackHandler;
        }
        
        private void PerformAttack(MonoBehaviourPun handle, InputAction.CallbackContext callback)
        {
            
            handle.StartCoroutine(AttackFunc(handle));
        }
        private IEnumerator AttackFunc(MonoBehaviourPun handle)
        {
            yield return new WaitForSeconds(timeBeforeHitbox);
            _collider.SetActive(true);
            yield return new WaitForSeconds(hitboxDuration);
            _collider.SetActive(false);
        }
    }
}