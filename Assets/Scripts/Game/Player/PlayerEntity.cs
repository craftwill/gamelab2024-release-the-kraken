using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.UIElements;
using Cinemachine;

namespace Kraken
{
    public class PlayerEntity : MonoBehaviourPun
    {
        private bool _isOwner;
        [SerializeField] private PlayerSoundComponent _soundComponent;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private GameObject _camera;
        [SerializeField] private Transform _cameraOrientation;
        private Vector2 _moveVec = Vector2.zero;
        private float _fallingVelocity = -1.0f;
        private bool _isSprinting = false;

        [SerializeField] private InputActionReference _sprintInput;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                _camera.SetActive(true);
                _camera.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = Config.current.xCameraSensitivity;
                _camera.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = Config.current.yCameraSensitivity;

                _moveInput.action.performed += OnMove;
                _moveInput.action.canceled += OnMove;
                _sprintInput.action.performed += OnSprintPerformed;
                _sprintInput.action.canceled += OnSprintCanceled;
            }
        }

        private void OnDestroy()
        {
            _sprintInput.action.performed -= OnSprintPerformed;
            _sprintInput.action.canceled -= OnSprintCanceled;
            _moveInput.action.performed -= OnMove;
            _moveInput.action.canceled -= OnMove;
        }

            private void Update()
        {
            if (_isOwner)
            {
                if (_controller.isGrounded)
                {
                    _fallingVelocity = -1.0f;
                }
                ApplyGravity();

                Vector3 cameraDirection = transform.position - new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z);
                _cameraOrientation.forward = cameraDirection.normalized;
                Vector3 movementDirection = _cameraOrientation.forward * _moveVec.y + _cameraOrientation.right * _moveVec.x;
                if (movementDirection != Vector3.zero)
                {
                    transform.forward = Vector3.Slerp(transform.forward, movementDirection.normalized, Time.deltaTime * Config.current.rotationSpeed);
                    movementDirection = new Vector3(transform.forward.x, _fallingVelocity, transform.forward.z);
                }
                else
                {
                    movementDirection.y += _fallingVelocity;
                }
                if (_isSprinting)
                {
                    _controller.Move(movementDirection * Config.current.sprintSpeed * Time.deltaTime);
                }
                else
                {
                    _controller.Move(movementDirection * Config.current.moveSpeed * Time.deltaTime);
                }

            }
        }
        
        public void ApplyGravity()
        {
            _fallingVelocity -= Config.current.gravity * Time.deltaTime;
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            if (_isOwner)
            {
                _moveVec = value.ReadValue<Vector2>();
            }                
        }

        public void OnSprintPerformed(InputAction.CallbackContext value)
        {
            if (_isOwner)
            {
                _isSprinting = true;
                _soundComponent.PlaySprintSFX();
            }
        }

        public void OnSprintCanceled(InputAction.CallbackContext value)
        {
            if (_isOwner)
            {
                _isSprinting = false;
            }
        }
    }
}
