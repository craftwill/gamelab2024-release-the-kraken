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
        public int damage;

        private Action<InputAction.CallbackContext> _inputCallBackHandler;
        
        public void Subscribe(MonoBehaviourPun handle)
        {
            _inputCallBackHandler = (context) => PerformAttack(handle, context);
            attackInput.action.performed += _inputCallBackHandler;
        }
        public void Unsubscribe()
        {
            attackInput.action.performed -= _inputCallBackHandler;
        }
        private Vector3 p = Vector3.zero;
        private void PerformAttack(MonoBehaviourPun handle, InputAction.CallbackContext callback)
        {
            Vector3 pos = handle.transform.position;
           
            RaycastHit hit;
            if (Physics.CapsuleCast(pos, pos + Vector3.up, 1, handle.transform.forward, out hit, 1f)) 
            {
                Debug.Log("here");
            };
        }
    }
}