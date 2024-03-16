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
    }
}