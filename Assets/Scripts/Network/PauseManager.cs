using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PauseManager : MonoBehaviourPun
    {
        public enum PauseState
        {
            Unpaused,
            PausedBySelf,
            PausedByOther
        }
        public PauseState _pauseState { get; private set; } = PauseState.Unpaused;
        [SerializeField] private PauseMenuManager _pauseMenuManager;

        private void Start()
        {
            Time.timeScale = 1;
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
            _pauseMenuManager.OnTogglePause();
        }

        public void QuitToMainMenu()
        {
            photonView.RPC(nameof(RPC_All_QuitToMainMenu), RpcTarget.All);
        }

        [PunRPC]
        public void RPC_All_QuitToMainMenu()
        {
            AnimateManager.GetInstance().ClearAllAnimations();
            PhotonNetwork.LeaveRoom();
        }
    }
}
