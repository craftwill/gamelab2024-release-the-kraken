using Bytes;
using Cinemachine;
using Kraken;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kraken
{
    public class PlayerControlsComponent : MonoBehaviourPun
    {
        private bool _isOwner;
        [SerializeField] private PlayerSoundComponent _soundComponent;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private GameObject _camera;
        [SerializeField] private Transform _cameraOrientation;
        [SerializeField] private PlayerInput _input;
        private string _currentScheme;
        private Vector2 _moveVec = Vector2.zero;
        private float _fallingVelocity = -1.0f;
        private bool _isSprinting = false;
        private float _movementMagnitude = 0.0f;

        [SerializeField] private InputActionReference _sprintInput;
        [SerializeField] private InputActionReference _pauseInput;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                _camera.SetActive(true);
                CinemachineFreeLook freeLookCam = _camera.GetComponent<CinemachineFreeLook>();
                if (_input.currentControlScheme.Equals("Gamepad"))
                {
                    freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.cameraControllerMultiplier;
                    freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier * Config.current.cameraControllerMultiplier;
                }
                else
                {
                    freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity;
                    freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier; ;
                }
                freeLookCam.m_XAxis.m_InvertInput = Config.current.invertXAxis;
                freeLookCam.m_YAxis.m_InvertInput = Config.current.invertYAxis;
                _currentScheme = _input.currentControlScheme;

                _moveInput.action.performed += OnMove;
                _moveInput.action.canceled += OnMove;
                _sprintInput.action.performed += OnSprintPerformed;
                _sprintInput.action.canceled += OnSprintCanceled;
                _pauseInput.action.performed += OnPause;

                EventManager.AddEventListener(EventNames.ToggleCursor, ToggleCursor);
            }
        }

        private void OnDestroy()
        {
            _sprintInput.action.performed -= OnSprintPerformed;
            _sprintInput.action.canceled -= OnSprintCanceled;
            _moveInput.action.performed -= OnMove;
            _moveInput.action.canceled -= OnMove;
            _pauseInput.action.performed -= OnPause;

            EventManager.RemoveEventListener(EventNames.ToggleCursor, ToggleCursor);
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
                    _controller.Move(movementDirection * Config.current.moveSpeed * _movementMagnitude * Time.deltaTime);
                }

                if (!_currentScheme.Equals(_input.currentControlScheme))
                {
                    OnControlsChanged(_input.currentControlScheme);
                    _currentScheme = _input.currentControlScheme;
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
                _movementMagnitude = Mathf.Clamp(_moveVec.magnitude, 0.0f, 1.0f);
            }
        }

        public void OnSprintPerformed(InputAction.CallbackContext value)
        {
            if (_isOwner)
            {
                _isSprinting = true;
            }
            photonView.RPC(nameof(_soundComponent.RPC_All_PlaySprintSound), RpcTarget.All);
        }

        public void OnSprintCanceled(InputAction.CallbackContext value)
        {
            if (_isOwner)
            {
                _isSprinting = false;
            }
        }

        public void OnPause(InputAction.CallbackContext value)
        {
            EventManager.Dispatch(EventNames.TogglePause, null);
        }

        private void ToggleCursor(BytesData data)
        {
            bool toggle = ((BoolDataBytes)data).BoolValue;
            Cursor.visible = toggle;
            Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void OnControlsChanged(string newScheme)
        {
            if (newScheme.Equals("Gamepad"))
            {
                _camera.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.cameraControllerMultiplier;
                _camera.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier * Config.current.cameraControllerMultiplier;
            }
            else
            {
                _camera.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity;
                _camera.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier; ;
            }
        }
    }
}
