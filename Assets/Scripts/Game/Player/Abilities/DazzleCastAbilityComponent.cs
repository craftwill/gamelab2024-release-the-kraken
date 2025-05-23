
using UnityEngine;
using UnityEngine.InputSystem;

using Kraken.Network;

using Bytes;
using Unity.VisualScripting;
using Photon.Pun;

namespace Kraken
{
    public class DazzleCastAbilityComponent : EntityCastAbilityComponent
    {
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private InputActionReference _castAbilityInput;
        [SerializeField] private GameObject _abilityPrefab;
        [SerializeField] private PlayerControlsComponent _controlsComponent;
        [SerializeField] private PlayerSoundComponent _soundComponent;
        [SerializeField] private PauseManager _pauseManager = null;
        [SerializeField] private float _verticalOffset = -0.25f;
        [SerializeField] private LayerMask _wallLayerMask;
        private bool _ultimateRunning = false;

        private float _jumpDuration = 0.27f;
        private float _jumpDistance = 7f;
        private float _jumpHeight = 3f;

        private void Start()
        {
            _cooldown = Config.current.dazzleAbilityCooldown;
            _jumpDuration = Config.current.dazzleAbilityJumpDuration;
            _jumpDistance = Config.current.dazzleAbilityJumpDistance;
            _jumpHeight = Config.current.dazzleAbilityJumpHeigth;
            if (_pauseManager == null)
            {
                _pauseManager = Object.FindObjectOfType<PauseManager>(); //temp but i don't wanna lock the game scene
            }
            EventManager.AddEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
        }

        private void Awake()
        {
            _castAbilityInput.action.performed += OnCastAbility;
        }

        private void OnDestroy()
        {
            _castAbilityInput.action.performed -= OnCastAbility;
            EventManager.RemoveEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
        }

        public void OnCastAbility(InputAction.CallbackContext value)
        {
            if (!_controlsComponent.controlsEnabled || _pauseManager.Paused || _ultimateRunning) return;
            CastAbility();
        }

        public override void CastAbility()
        {
            if (!CanCastAbility()) return;
            if (!photonView.AmOwner) return;

            base.CastAbility();

            JumpAnimation();
        }

        public void JumpAnimation() 
        {
            _playerEntity.SetControlsEnabled(false);

            Vector3 startPos = transform.position;
            Vector3 middlePos = transform.position + transform.forward * _jumpDistance / 2 + new Vector3(0f, _jumpHeight, 0f);
            Vector3 endPos = transform.position + transform.forward * _jumpDistance;

            // Raycast above endPos towards ground and see where the floor is.
            int groundLayerMask = LayerMask.GetMask("Terrain");
            Ray ray = new Ray(endPos + new Vector3(0, 20f, 0f), Vector3.down);
            if (Physics.Raycast(ray, out var hit, 10000f, groundLayerMask))
            {
                endPos = hit.point;
            }
            else
            {
                // If not impact, can't cast ability and cancel cooldown.
                CancelCooldown();
                return;
            }

            // Check for walls
            ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var wallHit, _jumpDistance, _wallLayerMask))
            {
                float height = middlePos.y;
                middlePos = wallHit.point;
                middlePos.y = height;
                ray = new Ray(middlePos, Vector3.down);
                if (Physics.Raycast(ray, out var hit2, 10000f, groundLayerMask))
                {
                    endPos = hit2.point;
                }
            }
            photonView.RPC(nameof(_soundComponent.RPC_All_PlayDazzleAbilityJumpSound), RpcTarget.All);

            Animate.LerpSomething(_jumpDuration, (float step) => 
            {  
                if (step < 0.5f)
                {
                    transform.position = Vector3.Slerp(startPos, middlePos, step * 2);
                }
                else
                {
                    transform.position = Vector3.Slerp(middlePos, endPos, (step - 0.5f) * 2f);
                }
            }, () =>
            {
                Vector3 spawnPos = transform.position + new Vector3(0f, _verticalOffset, 0f);
                GameObject ability = NetworkUtils.Instantiate(_abilityPrefab.name, spawnPos);

                DazzleAoEAbility aoeAbility = ability.GetComponent<DazzleAoEAbility>();
                aoeAbility.ActivateAbility();

                photonView.RPC(nameof(_soundComponent.RPC_All_PlayDazzleAbilityCrashSound), RpcTarget.All);
                _playerEntity.SetControlsEnabled(true);
                transform.position = endPos;
            }, timeScaled_: true);
        }

        private void HandleUltimateRunning(BytesData data)
        {
            _ultimateRunning = ((BoolDataBytes)data).BoolValue;
        }
    }
}