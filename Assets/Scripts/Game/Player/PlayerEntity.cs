
using UnityEngine;

using Photon.Pun;

using Bytes;

namespace Kraken
{
    public class PlayerEntity : Entity
    {
        [SerializeField] private PlayerControlsComponent _controls;
        [SerializeField] private PlayerSoundComponent _soundComponent;

        private PlayerAnimationComponent _playerAnimationComponent;
        public bool _isOwner { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            _healthComponent.MaxHealth = Config.current.maxHealth;
        }

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
                // Set above head canvas disabled if we are the local player
                GetComponentInChildren<PlayerAboveHeadIconComponent>().gameObject.SetActive(false);
            }

            _playerAnimationComponent = (PlayerAnimationComponent) _entityAnimationComponent;

            EventManager.AddEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventManager.RemoveEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        protected virtual void HandleStopGameFlow(BytesData data)
        {
            if (!_isOwner) return;

            _controls.SetControlsEnabled(false);
            _controls.SetCameraControlsEnabled(false);
            _controls.SetCameraEnabled(false);
        }

        protected override void HandleDie()
        {
            base.HandleDie();

            StringDataBytes bytes = new StringDataBytes(PhotonNetwork.LocalPlayer.UserId);
            EventManager.Dispatch(EventNames.PlayerDeath, bytes);
        }

        protected override void HandleTakeDamage(float dmgAmount)
        {
            base.HandleTakeDamage(dmgAmount);

            // Don't play hurt anim if player is attacking
            IKrakenAnimState currentPlayOnceAnim = _playerAnimationComponent.GetCurrentPlayOnceAnim();
            if (currentPlayOnceAnim != null && currentPlayOnceAnim.ClipName.Contains("AttackCombo"))
            {
                // Hurt without anim
            }
            else
            {
                _playerAnimationComponent.PlayHurtAnim();
            }

            var hpData = new FloatDataBytes(_healthComponent.Health / _healthComponent.MaxHealth);
            if (_isOwner)
            {
                EventManager.Dispatch(EventNames.UpdatePlayerHealthUI, hpData);
                photonView.RPC(nameof(_soundComponent.RPC_All_PlayPlayerHurtSound), RpcTarget.All);
            }
            else
            {
                EventManager.Dispatch(EventNames.UpdateOtherPlayerHealthUI, hpData);
            }
        }

        protected override void HandleHealed(float healAmount)
        {
            var hpData = new FloatDataBytes(_healthComponent.Health / _healthComponent.MaxHealth);
            if (_isOwner)
            {
                EventManager.Dispatch(EventNames.UpdatePlayerHealthUI, hpData);
            }
            else
            {
                EventManager.Dispatch(EventNames.UpdateOtherPlayerHealthUI, hpData);
            }
        }

        public void PlayAttackAnimationCombo(int comboStep, System.Action animDonePlayingCallback)
        {
            _playerAnimationComponent.PlayAttackAnimationCombo(comboStep, animDonePlayingCallback);

            photonView.RPC(nameof(RPC_Other_PlayAttackAnimationCombo), RpcTarget.Others, comboStep);
        }

        [PunRPC]
        private void RPC_Other_PlayAttackAnimationCombo(int comboStep) 
        {
            _playerAnimationComponent.PlayAttackAnimationCombo(comboStep);
        }

        public void SetControlsEnabled(bool controlsEnabled) 
        {
            _controls.SetControlsEnabled(controlsEnabled);
        }
    }
}
