using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.UIElements;

namespace Kraken
{
    public class PlayerEntity : MonoBehaviourPun
    {
        private bool _isOwner;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private GameObject _camera;
        [SerializeField] private Transform _cameraOrientation;
        private Vector2 _moveVec = Vector2.zero;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                _moveInput.action.performed += OnMove;
                _moveInput.action.canceled += OnMove;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
        }

        private void Update()
        {
            if (_isOwner)
            {
                Vector3 cameraDirection = transform­.position - new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z);
                _cameraOrientation.forward = cameraDirection.normalized;
                Vector3 movementDirection = _cameraOrientation.forward * _moveVec.y + _cameraOrientation.right * _moveVec.x;
                if (movementDirection != Vector3.zero)
                {
                    transform.forward = Vector3.Slerp(transform.forward, movementDirection.normalized, Time.deltaTime * Config.current.rotationSpeed);
                    transform.position += transform.forward * Config.current.moveSpeed * Time.deltaTime;
                    //_rigidBody.velocity = new Vector3(_moveVec.x, 0, _moveVec.y) * Config.current.moveSpeed * Time.deltaTime;
                }
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
