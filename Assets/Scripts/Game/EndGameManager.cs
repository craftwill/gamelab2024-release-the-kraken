using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytes;

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

        // Update is called once per frame
        void Update()
        {

        }

        private void HandleStartGameTimer(BytesData data)
        {
            Debug.Log("Starting game timer!");

            photonView.RPC(nameof(RPC_StartCountdownTimer), RpcTarget.All);
        }
    }
}