using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class RazzlePullAbilitySoundComponent : MonoBehaviourPun
    {
        [SerializeField] private AK.Wwise.Event _whirlpoolPlay;
        [SerializeField] private AK.Wwise.Event _whirlpoolStop;

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
        public void RPC_All_PlayWhirlpool()
        {
            _whirlpoolPlay.Post(gameObject);
        }

        [PunRPC]
        public void RPC_All_StopWhirlpool()
        {
            _whirlpoolStop.Post(gameObject);
        }
    }
}
