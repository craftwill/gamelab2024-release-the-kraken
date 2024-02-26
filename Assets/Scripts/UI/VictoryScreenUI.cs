using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bytes;

namespace Kraken.UI
{
    public class VictoryScreenUI : KrakenUIElement
    {
        private void Start()
        {
            EventManager.AddEventListener(EventNames.ShowVictoryScreenUI, HandleShowVictoryScreenUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowVictoryScreenUI, HandleShowVictoryScreenUI);
        }

        private void HandleShowVictoryScreenUI(BytesData data)
        {
            SetVisible(true);
        }
    }
}
