
using UnityEngine;
using UnityEngine.InputSystem;

using Kraken.Network;

namespace Kraken
{
    public class RazzleCastAbilityComponent : EntityCastAbilityComponent
    {
        [SerializeField] private InputActionReference _castAbilityInput;
        [SerializeField] private GameObject _abilityPrefab;
        [SerializeField] private float _verticalOffset = -0.35f;
        private float _spawnDistanceOffset = 5f;

        private void Start()
        {
            _spawnDistanceOffset = Config.current.razzleAbilitySpawnDistanceOffset;
            _cooldown = Config.current.razzleAbilityCooldown;
        }

        private void Awake()
        {
            _castAbilityInput.action.performed += OnCastAbility;
        }

        private void OnDestroy()
        {
            _castAbilityInput.action.performed -= OnCastAbility;
        }

        public void OnCastAbility(InputAction.CallbackContext value)
        {
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
        }
    }
}