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
            EventManager.AddEventListener(EventNames.ZoneFullLoss, HandleZoneFullLoss);
        }

        private void OnDestroy()
        {
            if (!_isMaster) return;

            EventManager.RemoveEventListener(EventNames.StartGameTimer, HandleStartGameTimer);
            EventManager.RemoveEventListener(EventNames.PlayerDeath, HandlePlayerDeath);
            EventManager.RemoveEventListener(EventNames.PlayerWin, HandlePlayerWin);
            EventManager.RemoveEventListener(EventNames.ZoneFullLoss, HandleZoneFullLoss);
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

        private void HandleZoneFullLoss(BytesData data)
        {
            EventManager.Dispatch(EventNames.StopGameFlow, null);
            EndGameAfterDefeat();
        }

        [PunRPC]
        public void RPC_StartGameTimer()
        {
            void GameTimerDoneCallback()
            {
                if (!_isMaster) return;
                EndGameAfterGameTimer();
            }
            
            Animate.Delay(Config.current.gameDuration, GameTimerDoneCallback, true);
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
            EndGame(isVictory: false);
        }

        private void EndGameAfterPlayerDeath()
        {
            EndGame(isVictory: false);
        }

        private void EndGameAfterWin()
        {
            EndGame(isVictory: true);
        }

        private void EndGameAfterDefeat()
        {
            EndGame(isVictory: false);
        }

        private void EndGame(bool isVictory)
        {
            EventManager.Dispatch(EventNames.ToggleCursor, new BoolDataBytes(true));

            if (isVictory)
            {
                EventManager.Dispatch(EventNames.ShowVictoryScreenUI, null);
            }
            else
            {
                EventManager.Dispatch(EventNames.ShowDefeatScreenUI, null);
            }
            
            GameManager.ToggleCursor(true);
        }
    }
}