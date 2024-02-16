using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PauseManager : MonoBehaviourPun
    {
        enum PauseState
        {
            Unpaused,
            PausedBySelf,
            PausedByOther
        }
        private PauseState _pauseState = PauseState.Unpaused;

        private void Start()
        {
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
            EventManager.AddEventListener(EventNames.TogglePause, TogglePause);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.TogglePause, TogglePause);
        }

        private void TogglePause(BytesData data)
        {
            if (_pauseState == PauseState.Unpaused)
            {
                _pauseState = PauseState.PausedBySelf;
                photonView.RPC(nameof(RPC_All_TogglePause), RpcTarget.All, true);
            }
            else if (_pauseState == PauseState.PausedBySelf)
            {
                photonView.RPC(nameof(RPC_All_TogglePause), RpcTarget.All, false);
            }
        }

        [PunRPC]
        public void RPC_All_TogglePause(bool pause)
        {
            if (pause)
            {
                Time.timeScale = 0;
                if (_pauseState == PauseState.Unpaused)
                {
                    _pauseState = PauseState.PausedByOther;
                }
            }
            else
            {
                Time.timeScale = 1;
                _pauseState = PauseState.Unpaused;
            }
        }
    }
}
