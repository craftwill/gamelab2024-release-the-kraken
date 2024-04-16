using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PauseManager : MonoBehaviourPun
    {
        [SerializeField] private PauseMenuManager _pauseMenuManager;
        public bool Paused { get; private set; } = false;

        private void Start()
        {
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
            EventManager.AddEventListener(EventNames.TogglePause, TogglePause);
            EventManager.AddEventListener(EventNames.StopGameFlow, DisablePause);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.TogglePause, TogglePause);
            EventManager.RemoveEventListener(EventNames.StopGameFlow, DisablePause);
        }

        private void TogglePause(BytesData data)
        {
            Paused = !Paused;
            _pauseMenuManager.OnTogglePause();
        }

        private void DisablePause(BytesData data)
        {
            EventManager.RemoveEventListener(EventNames.TogglePause, TogglePause);
            _pauseMenuManager.UnPause();
            Paused = false;
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
