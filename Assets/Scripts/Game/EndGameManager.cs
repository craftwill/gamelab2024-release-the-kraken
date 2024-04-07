using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytes;
using Photon.Pun;

namespace Kraken
{
    public enum EndGameType 
    {
        TimerOut,
        PlayerDeath,
        PlayerWin,
        ZoneFullLoss
    }
    public class EndGameManager : KrakenNetworkedManager
    {
        [SerializeField] private EndGameManagerSoundComponent _soundComponent;
        private bool isGameEnded = false;

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
            photonView.RPC(nameof(RPC_All_EndGame), RpcTarget.All, false, (int)EndGameType.PlayerDeath);
        }

        private void HandlePlayerWin(BytesData data)
        {
            photonView.RPC(nameof(RPC_All_EndGame), RpcTarget.All, true, (int)EndGameType.PlayerWin);
        }

        private void HandleZoneFullLoss(BytesData data)
        {
            print("DEFEAT!! -------- ZoneFullLoss");
            photonView.RPC(nameof(RPC_All_EndGame), RpcTarget.All, false, (int)EndGameType.ZoneFullLoss);
        }

        [PunRPC]
        public void RPC_StartGameTimer()
        {
            void GameTimerDoneCallback()
            {
                if (!_isMaster) return;
                print("DEFEAT!! -------- Timer expired!");
                photonView.RPC(nameof(RPC_All_EndGame), RpcTarget.All, false, (int)EndGameType.TimerOut);
            }

            //Config.current.gameDuration
            Animate timeLeftAnim = Animate.Delay(20f, GameTimerDoneCallback, true);
            // Send the timer Animate object to the UI to update it dynamically.
            EventManager.Dispatch(EventNames.InitTimeLeftUI, new ObjectDataBytes(timeLeftAnim));
        }


        [PunRPC]
        public void RPC_All_EndGame(bool isVictory, int endGameTypeInt)
        {
            EndGameType EndGameType = (EndGameType)endGameTypeInt;
            EndGame(isVictory, EndGameType);
        }

        private void EndGame(bool isVictory, EndGameType endGameType)
        {
            if (isGameEnded) return;

            isGameEnded = true;

            EventManager.Dispatch(EventNames.StopGameFlow, null);

            if (isVictory)
            {
                EventManager.Dispatch(EventNames.ShowVictoryScreenUI, null);
                _soundComponent.PlayVictorySound();
            }
            else
            {
                EventManager.Dispatch(EventNames.ShowDefeatScreenUI, null);
                _soundComponent.PlayDefeatSound();
            }

            switch (endGameType) 
            {
                case EndGameType.TimerOut:
                    EventManager.Dispatch(EventNames.ShowDefeatTimeLeftUI, null);
                    break;
            }

            GameManager.ToggleCursor(true);
        }
    }
}