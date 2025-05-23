
using UnityEngine;

using Photon.Pun;

using Bytes;

namespace Kraken
{
    public class GameFlowManager : KrakenNetworkedManager
    {
        private void Start()
        {
            //Stop sounds on scene load
            AkSoundEngine.StopAll();
            //scuffed
            CombatUtils.ResetGetPlayer();
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
            AnimateManager.GetInstance().ClearAllAnimations();
            //night increment done through the victory screen
            PhotonNetwork.LoadLevel("Game");
        }
    }
}