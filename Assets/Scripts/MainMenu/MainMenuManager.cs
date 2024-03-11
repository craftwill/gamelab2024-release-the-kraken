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

        [SerializeField] private TMP_InputField _inputFieldRoomCode;

        private void Start()
        {
            cvGroup = GetComponent<CanvasGroup>();

            // If already connected, allows menu interaction
            if (PhotonNetwork.IsConnected)
            {
                cvGroup.interactable = true;
            }
            _inputFieldRoomCode.onSubmit.AddListener(OnEnterInCodeField);
        }

        private void OnDestroy()
        {
            _inputFieldRoomCode.onSubmit.RemoveListener(OnEnterInCodeField);
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
            if (_inputFieldRoomCode.text.Length == 0) { return; }
            
            isJoining = true;
            EventManager.Dispatch(EventNames.JoinRoomWithCode, new StringDataBytes(_inputFieldRoomCode.text.ToUpper()));
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

        private void OnEnterInCodeField(string text)
        {
            Btn_OnJoinGame();
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning("Couldn't join: " + message);
            isJoining = false;
        }
    }
}
