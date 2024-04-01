using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kraken
{
    public class PauseMenuManager : MonoBehaviourPun
    {
        [SerializeField] private GameObject _pauseScreen;
        [SerializeField] private GameObject _gameCanvas;
        [SerializeField] private GameObject _pauseBaseMenu;
        [SerializeField] private GameObject _pauseSettingsMenu;
        [SerializeField] private PauseManager _pauseManager = null;
        private bool _paused = false;

        private void Start()
        {
            if (_pauseManager == null)
            {
                _pauseManager = Object.FindObjectOfType<PauseManager>(); //temp but i don't wanna lock the game scene
            }
        }

        public void OnTogglePause()
        {
            _paused = !_paused;
            _pauseScreen.SetActive(_paused);
            if (Config.current.hideGameCanvasOnPause)
            {
                _gameCanvas.SetActive(!_paused);
            }
            Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _paused;
            _pauseBaseMenu.SetActive(true);
            _pauseSettingsMenu.SetActive(false);
        }

        public void OnBtnResume()
        {
            EventManager.Dispatch(EventNames.TogglePause, null);
        }

        public void OnBtnSettings()
        {
            _pauseBaseMenu.SetActive(false);
            _pauseSettingsMenu.SetActive(true);
        }

        public void OnBtnQuit()
        {
            _pauseManager.QuitToMainMenu();
        }
    }
}