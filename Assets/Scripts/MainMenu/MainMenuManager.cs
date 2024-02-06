using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun;

using TMPro;

using Bytes;
using Kraken;

namespace Together
{
    public class MainMenuManager : MonoBehaviourPunCallbacks
    {
        private bool isHosting = false;
        private bool isJoining = false;
        private bool isQuitting = false;

        private CanvasGroup cvGroup;

        private void Start()
        {
            cvGroup = GetComponent<CanvasGroup>();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            if (!Config.current.isSkipMainMenu)
            {
                cvGroup.interactable = true;
            }
            else
            {
                // Skip main menu by hosting directly.
                Btn_OnHostGame();
            }
        }

        public void Btn_OnHostGame()
        {
            if (isHosting) { return; }

            isHosting = true;
            EventManager.Dispatch(EventNames.CreateRoom, null);
        }

        public void Btn_OnJoinGame()
        {
            if (isJoining) { return; }

            isJoining = true;
            EventManager.Dispatch(EventNames.JoinRoomWithCode, null);
        }

        public void Btn_OnQuit()
        {
            if (isQuitting) { return; }

            isQuitting = true;
            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            
            if(cause == DisconnectCause.DisconnectByClientLogic)
                Application.Quit();
        }
    }
}
