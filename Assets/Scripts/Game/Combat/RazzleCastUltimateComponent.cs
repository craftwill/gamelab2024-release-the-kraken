using Bytes;
using Kraken.Network;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kraken 
{
    public class RazzleCastUltimateComponent : EntityCastAbilityComponent
    {
        [SerializeField] private InputActionReference _castUltimateInput;
        [SerializeField] private GameObject _ultimatePrefab;
        [SerializeField] private float _spawnDistanceOffset = 5f;

        private void Awake()
        {
            _castUltimateInput.action.performed += OnCastUltimate;
        }

        private void OnDestroy()
        {
            _castUltimateInput.action.performed -= OnCastUltimate;
        }

        public void OnCastUltimate(InputAction.CallbackContext value)
        {
            CastAbility();
        }

        public override void CastAbility()
        {
            if (!CanCastAbility()) return;

            base.CastAbility();

            if (!photonView.AmOwner) return;

            Vector3 spawnPos = transform.position + transform.forward * _spawnDistanceOffset - new Vector3(0f, 0.3f, 0f);
            GameObject ability = NetworkUtils.Instantiate(_ultimatePrefab.name, spawnPos);

            RazzlePullAbility pullAbility = ability.GetComponent<RazzlePullAbility>();
            pullAbility.ActivateAbility();
        }
    }
}
