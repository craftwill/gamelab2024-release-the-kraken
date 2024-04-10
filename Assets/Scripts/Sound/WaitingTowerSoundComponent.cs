using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class WaitingTowerSoundComponent : MonoBehaviourPun
    {

        [SerializeField] private AK.Wwise.Event _buildSound;

        private void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
            photonView.RPC(nameof(RPC_All_PlayBuildSound), RpcTarget.All);
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
        public void RPC_All_PlayBuildSound()
        {
            _buildSound.Post(gameObject);
        }
    }
}
