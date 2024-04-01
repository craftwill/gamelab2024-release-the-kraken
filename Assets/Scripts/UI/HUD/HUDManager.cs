
using UnityEngine;

using Bytes;

namespace Kraken.UI
{
    public class HUDManager : MonoBehaviour
    {
        // All UI components with state that changes depending on player and control schema
        private PlayerUIComponent[] _playerUIComponents;

        private void Awake()
        {
            _playerUIComponents = GetComponentsInChildren<PlayerUIComponent>();

            EventManager.AddEventListener(EventNames.SetupHUD, HandleSetupHUD);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.SetupHUD, HandleSetupHUD);
        }

        private void HandleSetupHUD(BytesData data) 
        {
            var setupData = (data as SetupHUDData);

            foreach (var comp in _playerUIComponents) 
            {
                comp.Init(setupData.IsRazzle, setupData.IsKeyboard);
            }
        }
    }
}
