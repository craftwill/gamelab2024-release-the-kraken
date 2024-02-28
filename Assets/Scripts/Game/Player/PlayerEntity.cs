
using UnityEngine;

using Photon.Pun;

using Bytes;

namespace Kraken
{
    public class PlayerEntity : Entity
    {
        [SerializeField] private PlayerControlsComponent _controls;

        private PlayerAnimationComponent _playerAnimationComponent;
        private bool _isOwner;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
            }

            _playerAnimationComponent = (PlayerAnimationComponent) _entityAnimationComponent;

            EventManager.AddEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        protected virtual void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        protected virtual void HandleStopGameFlow(BytesData data)
        {
            print("Disable player controls!");
            _controls.DisableControls();
        }

        protected override void HandleDie()
        {
            base.HandleDie();

            StringDataBytes bytes = new StringDataBytes(PhotonNetwork.LocalPlayer.UserId);
            EventManager.Dispatch(EventNames.PlayerDeath, bytes);
        }

        public void PlayAttackAnimationCombo(int comboStep)
        {
            photonView.RPC(nameof(RPC_All_PlayAttackAnimationCombo), RpcTarget.All, comboStep);
        }

        [PunRPC]
        private void RPC_All_PlayAttackAnimationCombo(int comboStep) 
        {
            _playerAnimationComponent.PlayAttackAnimationCombo(comboStep);
        }
    }
}
