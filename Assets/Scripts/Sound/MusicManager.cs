using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MusicManager : MonoBehaviourPun
    {
        [SerializeField] private AK.Wwise.Event _playGeneralMusic;
        [SerializeField] private AK.Wwise.Event _stopGeneralMusic;
        [SerializeField] private AK.Wwise.Event _playBossMusic;
        [SerializeField] private AK.Wwise.Event _stopBossMusic;
        [SerializeField] private AK.Wwise.Event _playObjectiveMusic;
        [SerializeField] private AK.Wwise.Event _stopObjectiveMusic;

        private void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
            EventManager.AddEventListener(EventNames.StartGameFlow, HandleStartGameflow);
            EventManager.AddEventListener(EventNames.StopGameFlow, HandleStopGameflow);
            EventManager.AddEventListener(EventNames.BossSpawned, HandleBossSpawned);
        }

        private void OnDestroy()
        {
            AkSoundEngine.UnregisterGameObj(gameObject);
            EventManager.RemoveEventListener(EventNames.StartGameFlow, HandleStartGameflow);
            EventManager.RemoveEventListener(EventNames.StopGameFlow, HandleStopGameflow);
            EventManager.RemoveEventListener(EventNames.BossSpawned, HandleBossSpawned);
        }

        public void StopAllMusic()
        {
            photonView.RPC(nameof(RPC_All_StopGeneralMusic), RpcTarget.All);
            photonView.RPC(nameof(RPC_All_StopObjectiveMusic), RpcTarget.All);
            photonView.RPC(nameof(RPC_All_StopBossMusic), RpcTarget.All);
        }

        private void HandleStartGameflow(BytesData data)
        {
            photonView.RPC(nameof(RPC_All_PlayGeneralMusic), RpcTarget.All);
        }

        private void HandleStopGameflow(BytesData data)
        {
            StopAllMusic();
        }

        private void HandleBossSpawned(BytesData data)
        {
            StopAllMusic();
            photonView.RPC(nameof(RPC_All_PlayBossMusic), RpcTarget.All);

        }

        [PunRPC]
        private void RPC_All_PlayGeneralMusic()
        {
            _playGeneralMusic.Post(gameObject);
        }

        [PunRPC]
        private void RPC_All_StopGeneralMusic()
        {
            _stopGeneralMusic.Post(gameObject);
        }

        [PunRPC]
        private void RPC_All_PlayObjectiveMusic()
        {
            _playObjectiveMusic.Post(gameObject);
        }

        [PunRPC]
        private void RPC_All_StopObjectiveMusic()
        {
            _stopObjectiveMusic.Post(gameObject);
        }

        [PunRPC]
        private void RPC_All_PlayBossMusic()
        {
            _playBossMusic.Post(gameObject);
        }

        [PunRPC]
        private void RPC_All_StopBossMusic()
        {
            _stopBossMusic.Post(gameObject);
        }
    }

}
