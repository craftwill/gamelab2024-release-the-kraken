using System.Collections;
using System.Collections.Generic;
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

        private void HandleStartGameFlow(BytesData data)
        {
            Debug.Log("Starting gameflow!");

            void CountdownDoneCallback() 
            {
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
        }
    }
}