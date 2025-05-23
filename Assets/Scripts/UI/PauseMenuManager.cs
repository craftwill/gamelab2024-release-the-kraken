using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kraken
{
    public class PauseMenuManager : MonoBehaviourPun
    {
        [SerializeField] private GameObject _pauseScreen;
        [SerializeField] private GameObject _gameCanvas;
        [SerializeField] private GameObject _pauseBaseMenu;
        [SerializeField] private GameObject _pauseSettingsMenu;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private ControllerLayout _controllerLayout;
        [SerializeField] private ControllerLayout _keyboardLayout;
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
            _keyboardLayout.gameObject.SetActive(false);
            _controllerLayout.gameObject.SetActive(false);
            if (Config.current.hideGameCanvasOnPause)
            {
                _gameCanvas.SetActive(!_paused);
            }
            Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _paused;
            _pauseBaseMenu.SetActive(true);
            _pauseSettingsMenu.SetActive(false);
            if (_paused)
                _resumeButton.Select();
        }

        //when the game ends
        public void UnPause()
        {
            _paused = true;
            _pauseScreen.SetActive(false);
            GameManager.ToggleCursor(true);
            _pauseBaseMenu.SetActive(true);
            _pauseSettingsMenu.SetActive(false);
        }

        public void OnBtnResume()
        {
            EventManager.Dispatch(EventNames.TogglePause, null);
        }

        public void OnBtnControls()
        {
            void Back()
            {
                _pauseBaseMenu.SetActive(true);
            }

            _controllerLayout.gameObject.SetActive(true);
            _controllerLayout.SetBackButton(Back);
            _pauseBaseMenu.SetActive(false);
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