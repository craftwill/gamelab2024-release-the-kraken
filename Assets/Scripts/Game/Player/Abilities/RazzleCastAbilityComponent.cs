
using UnityEngine;
using UnityEngine.InputSystem;

using Kraken.Network;
using Bytes;
using Photon.Pun;

namespace Kraken
{
    public class RazzleCastAbilityComponent : EntityCastAbilityComponent
    {
        [SerializeField] private InputActionReference _castAbilityInput;
        [SerializeField] private PlayerControlsComponent _controlsComponent;
        [SerializeField] private PlayerSoundComponent _soundComponent;
        [SerializeField] private PauseManager _pauseManager = null;
        [SerializeField] private GameObject _abilityPrefab;
        [SerializeField] private float _verticalOffset = -0.35f;
        private float _spawnDistanceOffset = 5f;
        private bool _ultimateRunning = false;

        private void Start()
        {
            _spawnDistanceOffset = Config.current.razzleAbilitySpawnDistanceOffset;
            _cooldown = Config.current.razzleAbilityCooldown;
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

            Vector3 spawnPos = transform.position + transform.forward * _spawnDistanceOffset + new Vector3(0f, _verticalOffset, 0f);
            GameObject ability = NetworkUtils.Instantiate(_abilityPrefab.name, spawnPos);

            RazzlePullAbility pullAbility = ability.GetComponent<RazzlePullAbility>();
            pullAbility.ActivateAbility();
            photonView.RPC(nameof(_soundComponent.RPC_All_PlayRazzleAbilityCastSound), RpcTarget.All);
        }

        private void HandleUltimateRunning(BytesData data)
        {
            _ultimateRunning = ((BoolDataBytes)data).BoolValue;
        }
    }
}