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
        enum MovementState
        {
            Walking,
            Dashing,
            Sprinting,
            Attacking
        }
        private bool _isOwner;
        [SerializeField] private PlayerSoundComponent _soundComponent;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private GameObject _camera;
        [SerializeField] private Transform _cameraOrientation;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private DuoUltimateComponent _duoUltimateComponent;
        [SerializeField] private GameObject _takeDamageComponent;
        private MovementState _movementState = MovementState.Walking;
        private string _currentScheme;
        private Vector2 _moveVec = Vector2.zero;
        private float _fallingVelocity = -1.0f;
        private bool _sprintPressed = false;
        private float _movementMagnitude = 0.0f;
        private float _attackMovementSpeed = 0.0f;
        private bool _dashReady = true;

        [SerializeField] private InputActionReference _sprintInput;
        [SerializeField] private InputActionReference _pauseInput;
        [SerializeField] private InputActionReference _duoUltimateInput;

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
                _duoUltimateInput.action.performed += OnDuoUltimate;
                _duoUltimateInput.action.canceled += OnDuoUltimateReleased;

                GameManager.ToggleCursor(false);
                EventManager.AddEventListener(EventNames.PlayerAttackStart, HandleAttackStart);
                EventManager.AddEventListener(EventNames.PlayerAttackEnd, HandleAttackEnd);
            }
        }

        private void OnDestroy()
        {
            _sprintInput.action.performed -= OnSprintPerformed;
            _sprintInput.action.canceled -= OnSprintCanceled;
            _moveInput.action.performed -= OnMove;
            _moveInput.action.canceled -= OnMove;
            _pauseInput.action.performed -= OnPause;
            _duoUltimateInput.action.performed += OnDuoUltimate;
            _duoUltimateInput.action.canceled += OnDuoUltimateReleased;

            EventManager.RemoveEventListener(EventNames.PlayerAttackStart, HandleAttackStart);
            EventManager.RemoveEventListener(EventNames.PlayerAttackEnd, HandleAttackEnd);
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
                if (movementDirection != Vector3.zero && _movementState != MovementState.Attacking)
                {
                    transform.forward = Vector3.Slerp(transform.forward, movementDirection.normalized, Time.deltaTime * Config.current.rotationSpeed);
                }
                movementDirection = new Vector3(transform.forward.x, _fallingVelocity, transform.forward.z);
                if (_movementState == MovementState.Attacking)
                {
                    _controller.Move(transform.forward * _attackMovementSpeed * Time.deltaTime);
                }
                else if (_movementState == MovementState.Dashing)
                {
                    _controller.Move(movementDirection * Config.current.dashSpeed * Time.deltaTime);
                }
                else if (_movementState == MovementState.Sprinting)
                {
                    _controller.Move(movementDirection * Config.current.sprintSpeed * Time.deltaTime);
                }
                else
                {
                    movementDirection.x *= _movementMagnitude;
                    movementDirection.z *= _movementMagnitude;
                    _controller.Move(movementDirection * Config.current.moveSpeed * Time.deltaTime);
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
                _sprintPressed = true;
                if (_movementState != MovementState.Dashing)
                {
                    if (_dashReady)
                    {
                        StartCoroutine(DashCoroutine());
                        photonView.RPC(nameof(_soundComponent.RPC_All_PlaySprintSound), RpcTarget.All);
                    }
                    else
                    {
                        _movementState = MovementState.Sprinting;
                    }
                }
            }
        }

        public void OnSprintCanceled(InputAction.CallbackContext value)
        {
            if (_isOwner)
            {
                _sprintPressed = false;
                if (_movementState == MovementState.Sprinting)
                {
                    _movementState = MovementState.Walking;
                }
            }
        }

        IEnumerator DashCoroutine()
        {
            _movementState = MovementState.Dashing;
            _takeDamageComponent.SetActive(false);
            yield return new WaitForSeconds(Config.current.dashDuration);
            _movementState = _sprintPressed ? MovementState.Sprinting : MovementState.Walking;
            _takeDamageComponent.SetActive(true);
            StartCoroutine(DashCooldown());
        }

        IEnumerator DashCooldown()
        {
            _dashReady = false;
            yield return new WaitForSeconds(Config.current.dashCooldown);
            _dashReady = true;
        }

        public void OnPause(InputAction.CallbackContext value)
        {
            _camera.SetActive(!_camera.activeInHierarchy);
            EventManager.Dispatch(EventNames.TogglePause, null);
        }

        public void HandleAttackStart(BytesData data)
        {
            _attackMovementSpeed = ((FloatDataBytes)data).FloatValue;
            _movementState = MovementState.Attacking;
        }

        public void HandleAttackEnd(BytesData data)
        {
            if (_sprintPressed)
            {
                _movementState = MovementState.Sprinting;
            }
            else
            {
                _movementState = MovementState.Walking;
            }
        }

        public void OnControlsChanged(string newScheme)
        {
            CinemachineFreeLook freeLookCam = _camera.GetComponent<CinemachineFreeLook>();
            if (newScheme.Equals("Gamepad"))
            {
                freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.cameraControllerMultiplier;
                freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier * Config.current.cameraControllerMultiplier;
            }
            else
            {
                freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity;
                freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier;
            }
        }

        public void OnDuoUltimate(InputAction.CallbackContext value)
        {
            _duoUltimateComponent.OnDuoUltimateInput(true);
        }

        public void OnDuoUltimateReleased(InputAction.CallbackContext value)
        {
            _duoUltimateComponent.OnDuoUltimateInput(false);
        }
    }
}
