
using UnityEngine;
using UnityEngine.UI;

using Bytes;

namespace Kraken.UI
{
    public class PlayerPatchingUpUI : KrakenUIElement
    {
        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdatePatchingUpUI, HandleUpdatePatchingUpUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdatePatchingUpUI, HandleUpdatePatchingUpUI);
        }

        private void HandleUpdatePatchingUpUI(BytesData data) 
        {
            var showUI = (data as BoolDataBytes).BoolValue;

            SetVisible(showUI);
        }
    }
}
