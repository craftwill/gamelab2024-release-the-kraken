using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

namespace Kraken
{
    public class PlayerEntity : MonoBehaviourPun
    {
        private bool _isOwner;
        [SerializeField] private InputActionReference _moveInput;
        private Vector2 _moveVec = Vector2.zero;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                _moveInput.action.performed += OnMove;
                _moveInput.action.canceled += OnMove;
            }
        }

        private void Update()
        {
            if (_isOwner)
            {
                transform.position += new Vector3(_moveVec.x, 0, _moveVec.y) * Config.current.moveSpeed * Time.deltaTime;
            }
        }
        
        public void OnMove(InputAction.CallbackContext value)
        {
            if (_isOwner)
            {
                _moveVec = value.ReadValue<Vector2>();
            }                
        }
    }
}
