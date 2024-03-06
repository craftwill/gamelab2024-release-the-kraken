
using UnityEngine;
using UnityEngine.InputSystem;

using Kraken.Network;

using Bytes;

namespace Kraken
{
    public class DazzleCastAbilityComponent : EntityCastAbilityComponent
    {
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private InputActionReference _castAbilityInput;
        [SerializeField] private GameObject _abilityPrefab;
        [SerializeField] private float _verticalOffset = -0.25f;

        private float _jumpDuration = 0.27f;
        private float _jumpDistance = 7f;
        private float _jumpHeight = 3f;

        private void Start()
        {
            _cooldown = Config.current.dazzleAbilityCooldown;
            _jumpDuration = Config.current.dazzleAbilityJumpDuration;
            _jumpDistance = Config.current.dazzleAbilityJumpDistance;
            _jumpHeight = Config.current.dazzleAbilityJumpHeigth;
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

            JumpAnimation();
        }

        public void JumpAnimation() 
        {
            _playerEntity.SetControlsEnabled(false);

            Vector3 startPos = transform.position;
            Vector3 middlePos = transform.position + transform.forward * _jumpDistance / 2 + new Vector3(0f, _jumpHeight, 0f);
            Vector3 endPos = transform.position + transform.forward * _jumpDistance;
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

                DazzlePullAbility aoeAbility = ability.GetComponent<DazzlePullAbility>();
                aoeAbility.ActivateAbility();

                _playerEntity.SetControlsEnabled(true);
                transform.position = endPos;
            }, true);
        }
    }
}