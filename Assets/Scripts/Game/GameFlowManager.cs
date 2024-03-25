
using UnityEngine;

using Photon.Pun;

using Bytes;

namespace Kraken
{
    public class GameFlowManager : KrakenNetworkedManager
    {
        private void Start()
        {
            if (!_isMaster) return;

            EventManager.AddEventListener(EventNames.StartGameFlow, HandleStartGameFlow);
            EventManager.AddEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        private void OnDestroy()
        {
            if (!_isMaster) return;

            EventManager.RemoveEventListener(EventNames.StartGameFlow, HandleStartGameFlow);
            EventManager.RemoveEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        private void HandleStartGameFlow(BytesData data)
        {
            Debug.Log("Starting gameflow!");

            photonView.RPC(nameof(RPC_StartCountdownTimer), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_StartCountdownTimer()
        {
            void CountdownDoneCallback()
            {
                if (!_isMaster) return;
                StartGameAfterCountdown();
            }

            // Starts countdown before game starts!
            EventManager.Dispatch(EventNames.UpdateCountownTimerUI,
                new UpdateCountownTimerUIData(3f, CountdownDoneCallback));
        }

        private void HandleStopGameFlow(BytesData data)
        {
            Debug.Log("Stop gameflow!");

            EventManager.Dispatch(EventNames.StopObjectives, null);
            EventManager.Dispatch(EventNames.StopSpawning, null);
        }

        private void StartGameAfterCountdown()
        {
            // Start spawning enemies and activate objectives
            Debug.Log("Countdown done, start spawning enemies and set objective!");

            EventManager.Dispatch(EventNames.StartObjectives, null);
            EventManager.Dispatch(EventNames.StartSpawning, null);
            EventManager.Dispatch(EventNames.StartGameTimer, null);
        }

        public void GoToNextNight()
        {
            photonView.RPC(nameof(RPC_All_GoToNextNight), RpcTarget.All);
        }

        [PunRPC]
        public void RPC_All_GoToNextNight()
        {
            if (PlayerPrefs.HasKey(Config.GAME_NIGHT_KEY))
            {
                PlayerPrefs.SetInt(Config.GAME_NIGHT_KEY, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY) + 1);
            }
            else
            {
                PlayerPrefs.SetInt(Config.GAME_NIGHT_KEY, 1);
            }
            PhotonNetwork.LoadLevel("Game");
        }
    }
}