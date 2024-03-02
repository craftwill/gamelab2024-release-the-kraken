
using Bytes;
using UnityEngine;

namespace Kraken.UI
{
    public class VictoryScreenUI : BaseEndGameScrenUI
    {
        private void Start()
        {
            EventManager.AddEventListener(EventNames.ShowVictoryScreenUI, HandleShowScreenUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowVictoryScreenUI, HandleShowScreenUI);
        }
    }
}
