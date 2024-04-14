using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MusicManager : MonoBehaviourPun
    {
        enum MusicState { Menu, General, Objective, Boss};
        [SerializeField] private AK.Wwise.Event _playGeneralMusic;
        [SerializeField] private AK.Wwise.Event _stopGeneralMusic;
        [SerializeField] private AK.Wwise.Event _playBossMusic;
        [SerializeField] private AK.Wwise.Event _stopBossMusic;
        [SerializeField] private AK.Wwise.Event _playObjectiveMusic;
        [SerializeField] private AK.Wwise.Event _stopObjectiveMusic;
        [SerializeField] private AK.Wwise.Event _playMenuMusic;
        [SerializeField] private AK.Wwise.Event _stopMenuMusic;
        private MusicState _state = MusicState.General;
        private bool _inGame = false;
        private int _playersInZone = 0;

        private void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
            EventManager.AddEventListener(EventNames.StartGameFlow, HandleStartGameflow);
            EventManager.AddEventListener(EventNames.StopGameFlow, HandleStopGameflow);
            EventManager.AddEventListener(EventNames.BossSpawned, HandleBossSpawned);
            EventManager.AddEventListener(EventNames.PlayerEnteredObjective, HandlePlayerEnteredObjective);
            EventManager.AddEventListener(EventNames.PlayerLeftObjective, HandlePlayerLeftZone);
            EventManager.AddEventListener(EventNames.EnterMenu, HandleEnterMenu);
        }

        private void OnDestroy()
        {
            AkSoundEngine.UnregisterGameObj(gameObject);
            EventManager.RemoveEventListener(EventNames.StartGameFlow, HandleStartGameflow);
            EventManager.RemoveEventListener(EventNames.StopGameFlow, HandleStopGameflow);
            EventManager.RemoveEventListener(EventNames.BossSpawned, HandleBossSpawned);
            EventManager.RemoveEventListener(EventNames.PlayerEnteredObjective, HandlePlayerEnteredObjective);
            EventManager.RemoveEventListener(EventNames.PlayerLeftObjective, HandlePlayerLeftZone);
            EventManager.RemoveEventListener(EventNames.EnterMenu, HandleEnterMenu);
        }

        [PunRPC]
        public void RPC_All_StopAllMusic()
        {
            RPC_All_StopGeneralMusic();
            RPC_All_StopObjectiveMusic();
            RPC_All_StopBossMusic();
        }

        private void HandleStartGameflow(BytesData data)
        {
            _stopMenuMusic.Post(gameObject);
            photonView.RPC(nameof(RPC_All_PlayGeneralMusic), RpcTarget.All);
            _state = MusicState.General;
            _inGame = true;
        }

        private void HandleStopGameflow(BytesData data)
        {
            photonView.RPC(nameof(RPC_All_StopAllMusic), RpcTarget.All);
            _inGame = false;
        }

        private void HandleBossSpawned(BytesData data)
        {
            photonView.RPC(nameof(RPC_All_StopAllMusic), RpcTarget.All);
            photonView.RPC(nameof(RPC_All_PlayBossMusic), RpcTarget.All);
            _state = MusicState.Boss;
        }

        private void HandlePlayerEnteredObjective(BytesData data)
        {
            _playersInZone++;
            if (_state == MusicState.Boss || _state == MusicState.Objective || !Config.current.useObjectiveMusic || !_inGame) return;
            photonView.RPC(nameof(RPC_All_StopAllMusic), RpcTarget.All);
            photonView.RPC(nameof(RPC_All_PlayObjectiveMusic), RpcTarget.All);
            _state = MusicState.Objective;
        }

        private void HandlePlayerLeftZone(BytesData data)
        {
            _playersInZone = Mathf.Clamp(_playersInZone - 1, 0, _playersInZone);
            if (_state == MusicState.Boss || _state == MusicState.General || !Config.current.useObjectiveMusic || !_inGame) return;
            if (_playersInZone > 0) return;
            photonView.RPC(nameof(RPC_All_StopAllMusic), RpcTarget.All);
            photonView.RPC(nameof(RPC_All_PlayGeneralMusic), RpcTarget.All);
            _state = MusicState.General;
        }

        private void HandleEnterMenu(BytesData data)
        {
            if (_state != MusicState.Menu)
            {
                RPC_All_StopAllMusic();
                _playMenuMusic.Post(gameObject);
                _state = MusicState.Menu;
            }
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
