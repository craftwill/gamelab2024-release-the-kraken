using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerSoundComponent : MonoBehaviourPun
    {
        [SerializeField] private AK.Wwise.Event _sprintSound;
        [SerializeField] private AK.Wwise.Event _hurtSound;
        [SerializeField] private AK.Wwise.Event _ultimateGoOffSound;
        [SerializeField] private AK.Wwise.Event _ultimateReadySound;
        [SerializeField] private AK.Wwise.Event _ultimateNoticeSound;
        [SerializeField] private AK.Wwise.Event _ultimateTriggeredSound;
        [SerializeField] private AK.Wwise.Event _healingSoundStart;
        [SerializeField] private AK.Wwise.Event _healingSoundStop;
        [SerializeField] private AK.Wwise.Event _footstepSound;
        [SerializeField] private AK.Wwise.Event _dazzleAbilityJumpSound;
        [SerializeField] private AK.Wwise.Event _dazzleAbilityCrashSound;
        [SerializeField] private AK.Wwise.Event _razzleAbilityCastSound;
        [SerializeField] private AK.Wwise.Event _towerWaitingSound;

        private void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
        }

        private void OnDestroy()
        {
            AkSoundEngine.UnregisterGameObj(gameObject);
        }

        private void Update()
        {
            AkSoundEngine.SetObjectPosition(gameObject, transform);
        }

        [PunRPC]
        public void RPC_All_PlaySprintSound()
        {
            _sprintSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_PlayPlayerHurtSound()
        {
            _hurtSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_PlayUltimateGoOffSound()
        {
            _ultimateGoOffSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_PlayUltimateReady()
        {
            _ultimateReadySound.Post(gameObject);
        }

        public void PlayUltimateNoticeSound()
        {
            _ultimateNoticeSound.Post(gameObject);
        }

        public void PlayUltimateTriggeredSound()
        {
            _ultimateTriggeredSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_PlayHealingSound()
        {
            _healingSoundStart.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_StopHealingSound()
        {
            _healingSoundStop.Post(gameObject);
        }

        public void PlayFootstep()
        {
            photonView.RPC(nameof(RPC_All_PlayFootstepSound), RpcTarget.All);
        }

        [PunRPC]
        public void RPC_All_PlayFootstepSound()
        {
            _footstepSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_PlayDazzleAbilityJumpSound()
        {
            _dazzleAbilityJumpSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_PlayDazzleAbilityCrashSound()
        {
            _dazzleAbilityCrashSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_PlayRazzleAbilityCastSound()
        {
            _razzleAbilityCastSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_Other_PlayTowerWaitingSound()
        {
            _towerWaitingSound.Post(gameObject);
        }
    }
}