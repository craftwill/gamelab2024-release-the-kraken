using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class EnemySoundComponent : MonoBehaviourPun
    {
        [SerializeField] private AK.Wwise.Event _attackSound;
        [SerializeField] private AK.Wwise.Event _hurtSound;

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
        public void RPC_All_PlayEnemyAttackSound()
        {
            _attackSound.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_PlayEnemyHurtSound()
        {
            _hurtSound.Post(gameObject);
        }
    }
}