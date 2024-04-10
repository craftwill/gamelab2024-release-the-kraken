using Bytes;
using Cinemachine;
using Photon.Pun;
using System.Collections;
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
        [SerializeField] private PlayerAnimationComponent _playerAnimationComponent;
        [SerializeField] private PlayerAttackComponent _playerAttackComponent;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private GameObject _camera;
        [SerializeField] private Transform _cameraOrientation;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private DuoUltimateComponent _duoUltimateComponent;
        [SerializeField] private PlayerHealingComponent _healingComponent;
        [SerializeField] private PlayerTowerInteractComponent _towerInteractComponent;
        [SerializeField] private GameObject _takeDamageComponent;
        [SerializeField] private PauseManager _pauseManager = null;
        private CinemachineFreeLook _freeLookCam;
        
        private MovementState _movementState = MovementState.Walking;
        private string _currentScheme;
        private Vector2 _moveVec = Vector2.zero;
        private float _fallingVelocity = -1.0f;
        private bool _sprintPressed = false;
        private float _movementMagnitude = 0.0f;
        private float _attackMovementSpeed = 0.0f;
        private bool _dashReady = true;
        private Coroutine _resumeSprintCoroutine = null;

        public bool controlsEnabled { get; private set; } = true;
        private bool cameraControlsEnabled = true;
        private bool _ultimateRunning = false;

        [SerializeField] private InputActionReference _sprintInput;
        [SerializeField] private InputActionReference _pauseInput;
        [SerializeField] private InputActionReference _duoUltimateInput;
        [SerializeField] private InputActionReference _healInput;
        [SerializeField] private InputActionReference _towerInteractInput;

        private void Start()
        {
            _freeLookCam = _camera.GetComponent<CinemachineFreeLook>();
            if (_pauseManager == null)
            {
                _pauseManager = Object.FindObjectOfType<PauseManager>(); //temp but i don't wanna lock the game scene
            }

            if (photonView.AmOwner)
            {
                _isOwner = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _camera.SetActive(true);
                if (_input.currentControlScheme.Equals("Gamepad"))
                {
                    _freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.cameraControllerMultiplier;
                    _freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier * Config.current.cameraControllerMultiplier;
                }
                else
                {
                    _freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity;
                    _freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier; ;
                }
                _freeLookCam.m_XAxis.m_InvertInput = Config.current.invertXAxis;
                _freeLookCam.m_YAxis.m_InvertInput = Config.current.invertYAxis;
                _freeLookCam.m_Lens.FieldOfView = Config.current.baseFov;
                _currentScheme = _input.currentControlScheme;

                _moveInput.action.performed += OnMove;
                _moveInput.action.canceled += OnMove;
                _sprintInput.action.performed += OnSprintPerformed;
                _sprintInput.action.canceled += OnSprintCanceled;
                _pauseInput.action.performed += OnPause;
                _duoUltimateInput.action.performed += OnDuoUltimate;
                _healInput.action.performed += OnHeal;
                _healInput.action.canceled += OnHealReleased;
                _towerInteractInput.action.performed += OnTowerInteractPressed;
                _towerInteractInput.action.canceled += OnTowerInteractCanceled;

                GameManager.ToggleCursor(false);
                EventManager.AddEventListener(EventNames.PlayerAttackStart, HandleAttackStart);
                EventManager.AddEventListener(EventNames.PlayerAttackEnd, HandleAttackEnd);
                EventManager.AddEventListener(EventNames.UpdateCameraSettings, HandleCameraSettingsChanged);
                EventManager.AddEventListener(EventNames.TogglePause, OnTogglePause);
                EventManager.AddEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
            }
            else
            {
                _input.enabled = false;
            }
    
        }

        private void OnDestroy()
        {
            _sprintInput.action.performed -= OnSprintPerformed;
            _sprintInput.action.canceled -= OnSprintCanceled;
            _moveInput.action.performed -= OnMove;
            _moveInput.action.canceled -= OnMove;
            _pauseInput.action.performed -= OnPause;
            _duoUltimateInput.action.performed -= OnDuoUltimate;
            _healInput.action.performed -= OnHeal;
            _healInput.action.canceled -= OnHealReleased;

            EventManager.RemoveEventListener(EventNames.PlayerAttackStart, HandleAttackStart);
            EventManager.RemoveEventListener(EventNames.PlayerAttackEnd, HandleAttackEnd);
            EventManager.RemoveEventListener(EventNames.UpdateCameraSettings, HandleCameraSettingsChanged);
            EventManager.RemoveEventListener(EventNames.TogglePause, OnTogglePause);
            EventManager.RemoveEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
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

                if (cameraControlsEnabled)
                    ProcessCameraControls();

                if (controlsEnabled && !_pauseManager.Paused)
                    ProcessControls();

                //Workaround
                if (float.IsNaN(_freeLookCam.m_YAxis.Value))
                {
                    Debug.LogWarning("Y axis is NaN");
                    _freeLookCam.m_YAxis.Value = 0;
                }
            }
        }

        private void ProcessCameraControls() 
        {
            Vector3 cameraDirection = transform.position - new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z);
            _cameraOrientation.forward = cameraDirection.normalized;
            if (Config.current.changeFovOnSprint)
            {
                if (_movementState == MovementState.Sprinting && _movementMagnitude > 0.0f)
                {
                    _freeLookCam.m_Lens.FieldOfView = Mathf.Lerp(_freeLookCam.m_Lens.FieldOfView, Config.current.sprintFov, 1 / Config.current.fovChangeDuration);
                }
                else
                {
                    _freeLookCam.m_Lens.FieldOfView = Mathf.Lerp(_freeLookCam.m_Lens.FieldOfView, Config.current.baseFov, 1 / Config.current.fovChangeDuration);
                }
            }
        }

        private void ProcessControls() 
        {
            Vector3 movementDirection = _cameraOrientation.forward * _moveVec.y + _cameraOrientation.right * _moveVec.x;
            if (movementDirection != Vector3.zero && _movementState != MovementState.Attacking)
            {
                transform.forward = Vector3.Slerp(transform.forward, movementDirection.normalized, Time.deltaTime * Config.current.rotationSpeed);
            }
            movementDirection = new Vector3(0, _fallingVelocity, 0);
            if (_moveVec != Vector2.zero)
            {
                movementDirection.x = transform.forward.x;
                movementDirection.z = transform.forward.z;
            }
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

        public void ApplyGravity()
        {
            _fallingVelocity -= Config.current.gravity * Time.deltaTime;
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            if (!controlsEnabled || _pauseManager.Paused) return;

            if (_isOwner)
            {
                _moveVec = value.ReadValue<Vector2>();
                _movementMagnitude = Mathf.Clamp(_moveVec.magnitude, 0.0f, 1.0f);

                // Show correct moving animation and sync with other clients when needed.
                bool didAnimStateChange = false;
                if (Mathf.Abs(_moveVec.x) > 0f || Mathf.Abs(_moveVec.y) > 0f)
                {
                    didAnimStateChange = _playerAnimationComponent.SetLoopedStateWalking();
                    if (didAnimStateChange)
                        photonView.RPC(nameof(RPC_Other_SetLoopAnimState), RpcTarget.Others, "Walk");
                }
                else
                {
                    didAnimStateChange = _playerAnimationComponent.SetLoopedStateIdle();
                    if(didAnimStateChange) 
                        photonView.RPC(nameof(RPC_Other_SetLoopAnimState), RpcTarget.Others, "Idle");
                }
            }
        }

        [PunRPC]
        private void RPC_Other_SetLoopAnimState(string animStateType)
        {
            switch (animStateType)
            {
                case "Walk":  _playerAnimationComponent.SetLoopedStateWalking(); break;
                case "Idle":  _playerAnimationComponent.SetLoopedStateIdle(); break;
            }
        }

        public void OnSprintPerformed(InputAction.CallbackContext value)
        {
            if (!controlsEnabled || _pauseManager.Paused) return;

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
            if (!controlsEnabled || _pauseManager.Paused) return;

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
            EventManager.Dispatch(EventNames.TogglePause, null);
        }

        public void OnTogglePause(BytesData data)
        {
            _camera.SetActive(!_camera.activeInHierarchy);
            _moveVec = Vector2.zero;
            _movementState = MovementState.Walking;
            bool didAnimStateChange = _playerAnimationComponent.SetLoopedStateIdle();
            if (didAnimStateChange)
                photonView.RPC(nameof(RPC_Other_SetLoopAnimState), RpcTarget.Others, "Idle");
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
                _movementState = MovementState.Walking;
                if (_resumeSprintCoroutine != null)
                {
                    StopCoroutine(_resumeSprintCoroutine);
                    _resumeSprintCoroutine = null;
                }
                _resumeSprintCoroutine = StartCoroutine(resumeSprintingCoroutine());
            }
            else
            {
                _movementState = MovementState.Walking;
            }
        }

        private IEnumerator resumeSprintingCoroutine()
        {
            yield return new WaitForSeconds(Config.current.sprintAfterAttackCooldown);
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
            if (newScheme.Equals("Gamepad"))
            {
                _freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.cameraControllerMultiplier;
                _freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier * Config.current.cameraControllerMultiplier;
            }
            else
            {
                _freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity;
                _freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier;
            }

            EventManager.Dispatch(EventNames.InputSchemeChanged, new StringDataBytes(newScheme));
        }

        public void OnDuoUltimate(InputAction.CallbackContext value)
        {
            if (!controlsEnabled || _pauseManager.Paused) return;

            _duoUltimateComponent.OnDuoUltimateInput(true);
        }

        public void OnDuoUltimateReleased(InputAction.CallbackContext value)
        {
            if (!controlsEnabled || _pauseManager.Paused) return;

            _duoUltimateComponent.OnDuoUltimateInput(false);
        }

        public void OnHeal(InputAction.CallbackContext value)
        {
            if (!controlsEnabled || _pauseManager.Paused || _ultimateRunning) return;

            _healingComponent.OnHealingInput(true);
        }

        public void OnHealReleased(InputAction.CallbackContext value)
        {
            if (_pauseManager.Paused) return;
            _healingComponent.OnHealingInput(false);
        }

        public void OnTowerInteractPressed(InputAction.CallbackContext value)
        {
            if (!controlsEnabled || _pauseManager.Paused || _ultimateRunning) return;

            _towerInteractComponent.OnTowerInteractPressed();
        }

        public void OnTowerInteractCanceled(InputAction.CallbackContext value)
        {
            if (_pauseManager.Paused) return;

            _towerInteractComponent.OnTowerInteractCanceled();
        }

        public void SetControlsEnabled(bool controlsEnabled)
        {
            this.controlsEnabled = controlsEnabled;

            _playerAttackComponent.SetAttacksControlsEnabled(controlsEnabled);
        }

        public void SetCameraControlsEnabled(bool cameraControlsEnabled)
        {
            this.cameraControlsEnabled = cameraControlsEnabled;
        }

        public void SetCameraEnabled(bool isCameraEnabled) 
        {
            _freeLookCam.enabled = isCameraEnabled;
        }

        public void HandleCameraSettingsChanged(BytesData data)
        {
            if (_input.currentControlScheme.Equals("Gamepad"))
            {
                _freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.cameraControllerMultiplier;
                _freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier * Config.current.cameraControllerMultiplier;
            }
            else
            {
                _freeLookCam.m_XAxis.m_MaxSpeed = Config.current.cameraSensitivity;
                _freeLookCam.m_YAxis.m_MaxSpeed = Config.current.cameraSensitivity * Config.current.yCameraSensitivityMultiplier; ;
            }
            _freeLookCam.m_XAxis.m_InvertInput = Config.current.invertXAxis;
            _freeLookCam.m_YAxis.m_InvertInput = Config.current.invertYAxis;
            _freeLookCam.m_Lens.FieldOfView = Config.current.baseFov;
        }

        private void HandleUltimateRunning(BytesData data)
        {
            _ultimateRunning = ((BoolDataBytes)data).BoolValue;
        }
    }
}
