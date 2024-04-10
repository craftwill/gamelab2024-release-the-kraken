
using UnityEngine;

using Bytes;

namespace Kraken.UI
{
    public class HUDManager : MonoBehaviour
    {
        // All UI components with state that changes depending on player and control schema
        private IPlayerUIComponent[] _playerUIComponents;
        private PlayerProfileUI _playerProfileUI;

        private bool _isClientRazzle;

        private void Awake()
        {
            _playerUIComponents = GetComponentsInChildren<IPlayerUIComponent>();
            _playerProfileUI = GetComponentInChildren<PlayerProfileUI>();
        }

        private void Start()
        {
            EventManager.AddEventListener(EventNames.SetupHUD, HandleSetupHUD);
            EventManager.AddEventListener(EventNames.InputSchemeChanged, HandleInputSchemeChanged);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.SetupHUD, HandleSetupHUD);
            EventManager.RemoveEventListener(EventNames.InputSchemeChanged, HandleInputSchemeChanged);
        }

        private void HandleSetupHUD(BytesData data) 
        {
            var setupData = (data as SetupHUDData);

            _isClientRazzle = setupData.IsRazzle;
            UpdatePlayerUIComponents(setupData.IsRazzle, setupData.IsKeyboard);
            _playerProfileUI.Init(setupData.IsRazzle);
        }

        public void HandleInputSchemeChanged(BytesData data)
        {
            var newScheme = (data as StringDataBytes).StringValue;
            bool isKeyboard = !newScheme.Equals("Gamepad");
            UpdatePlayerUIComponents(_isClientRazzle, isKeyboard);
        }

        private void UpdatePlayerUIComponents(bool isRazzle, bool isKeyboard)
        {
            foreach (var comp in _playerUIComponents)
            {
                comp.Init(isRazzle, isKeyboard);
            }
        }
    }
}
