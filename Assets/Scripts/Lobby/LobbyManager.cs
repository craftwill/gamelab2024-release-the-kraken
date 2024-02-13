using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun;

using TMPro;

using Bytes;

namespace Kraken
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI txt_playerList;

        private void Start()
        {
            if (Config.current.isSkipMainMenu)
            {
                Btn_OnStartGame();
            }
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            photonView.RPC(nameof(RPC_All_UpdateLobbyView), RpcTarget.All);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            photonView.RPC(nameof(RPC_All_UpdateLobbyView), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_All_UpdateLobbyView() 
        {
            LocalUpdateLobbyView();
        }

        private void LocalUpdateLobbyView()
        {
            txt_playerList.text = "";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];

                txt_playerList.text += player.NickName;
                if (player.UserId == PhotonNetwork.LocalPlayer.UserId)
                {
                    txt_playerList.text += " (YOU)";
                }

                txt_playerList.text += "\n";
            }
        }

        public void Btn_OnStartGame() 
        {
            photonView.RPC(nameof(RPC_Master_OpenGameScene), RpcTarget.MasterClient);
        }

        /// <summary>
        /// RPC event called by Master Client to load the game scene
        /// </summary>
        [PunRPC]
        private void RPC_Master_OpenGameScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // MasterClient is like: yeah, sure, you can join!
                //photonView.RPC(nameof(RPC_JoinGameScene), RpcTarget.All);
                JoinGameScene();
            }
        }

        private void JoinGameScene()
        {
            var customSceneName = Config.current.loadCustomSceneName;
            if (customSceneName != "")
            {
                Debug.LogWarning("You loaded a custom scene with name: " + customSceneName);
                PhotonNetwork.LoadLevel(customSceneName);
                return;
            }
            PhotonNetwork.LoadLevel("Game");
        }
    }
}
