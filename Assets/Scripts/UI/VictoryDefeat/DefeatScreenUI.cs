
using Bytes;
using UnityEngine;

namespace Kraken.UI
{
    public class DefeatScreenUI : BaseEndGameScrenUI
    {
        private void Start()
        {
            EventManager.AddEventListener(EventNames.ShowDefeatScreenUI, HandleShowScreenUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowDefeatScreenUI, HandleShowScreenUI);
        }
    }
}
