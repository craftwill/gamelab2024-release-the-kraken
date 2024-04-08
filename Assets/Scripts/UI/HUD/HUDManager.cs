
using UnityEngine;

using Bytes;

namespace Kraken.UI
{
    public class HUDManager : KrakenUIElement
    {
        // All UI components with state that changes depending on player and control schema
        private PlayerUIComponent[] _playerUIComponents;
        private PlayerProfileUI _playerProfileUI;

        private void Awake()
        {
            _playerUIComponents = GetComponentsInChildren<PlayerUIComponent>();
            _playerProfileUI = GetComponentInChildren<PlayerProfileUI>();

            EventManager.AddEventListener(EventNames.SetupHUD, HandleSetupHUD);
            EventManager.AddEventListener(EventNames.HideHUD, HandleHideHUD);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.SetupHUD, HandleSetupHUD);
            EventManager.RemoveEventListener(EventNames.HideHUD, HandleHideHUD);
        }

        private void HandleSetupHUD(BytesData data) 
        {
            var setupData = (data as SetupHUDData);

            foreach (var comp in _playerUIComponents) 
            {
                comp.Init(setupData.IsRazzle, setupData.IsKeyboard);
            }

            _playerProfileUI.Init(setupData.IsRazzle);
        }

        private void HandleHideHUD(BytesData data)
        {
            SetVisible(false);
        }
    }
}
