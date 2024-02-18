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
        }

        private void OnDestroy()
        {
            if (!_isMaster) return;

            EventManager.RemoveEventListener(EventNames.StartGameTimer, HandleStartGameTimer);
        }

        private void HandleStartGameTimer(BytesData data)
        {
            Debug.Log("Starting game timer!");
            
            photonView.RPC(nameof(RPC_StartGameTimer), RpcTarget.All);
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
            Animate.Delay(5, GameTimerDoneCallback);
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
            //Go back to the lobby with second player
            //EventManager.Dispatch(EventNames.CreateRoom, null);
        }
    }
}