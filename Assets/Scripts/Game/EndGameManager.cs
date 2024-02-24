using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytes;
using Photon.Pun;

namespace Kraken
{
    public class EndGameManager : KrakenNetworkedManager
    {
        private void Start()
        {
            if (!_isMaster) return;

            EventManager.AddEventListener(EventNames.StartGameTimer, HandleStartGameTimer);
            EventManager.AddEventListener(EventNames.PlayerDeath, HandlePlayerDeath);
            EventManager.AddEventListener(EventNames.PlayerWin, HandlePlayerWin);
        }

        private void OnDestroy()
        {
            if (!_isMaster) return;

            EventManager.RemoveEventListener(EventNames.StartGameTimer, HandleStartGameTimer);
            EventManager.RemoveEventListener(EventNames.PlayerDeath, HandlePlayerDeath);
        }

        private void HandleStartGameTimer(BytesData data)
        {
            
            photonView.RPC(nameof(RPC_StartGameTimer), RpcTarget.All);
        }

        private void HandlePlayerDeath(BytesData data)
        {
            EventManager.Dispatch(EventNames.StopGameFlow, null);
            string playerId = (data as StringDataBytes).StringValue;

            photonView.RPC(nameof(RPC_PlayerDeath), RpcTarget.All, playerId);
        }

        private void HandlePlayerWin(BytesData data)
        {
            EventManager.Dispatch(EventNames.StopGameFlow, null);

            EndGameAfterWin();
        }

        [PunRPC]
        public void RPC_StartGameTimer()
        {
            void GameTimerDoneCallback()
            {
                if (!_isMaster) return;
                EndGameAfterGameTimer();
            }

            //EventManager.Dispatch(EventNames.UpdateGameTimerUI,
            //    new UpdateCountownTimerUIData(Config.current.gameDuration, GameTimerDoneCallback));
            Animate.Delay(Config.current.gameDuration, GameTimerDoneCallback);
        }

        [PunRPC]
        public void RPC_PlayerDeath(string playerId)
        {
            //to be implemented, depending on who died you show different thing to the player
            //somewhere in here we transition to a result scene

            if (!_isMaster) return;
            EndGameAfterPlayerDeath();
        }

        private void EndGameAfterGameTimer()
        {
            EndGame();
        }

        private void EndGameAfterPlayerDeath()
        {
            EndGame();
        }

        private void EndGameAfterWin()
        {
            EndGame();
        }

        private void EndGameAfterDefeat()
        {
            EndGame();
        }

        private void EndGame()
        {
            GameManager.ToggleCursor(true);
            
            PhotonNetwork.LoadLevel("Lobby");
        }
    }
}