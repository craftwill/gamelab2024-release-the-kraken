using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kraken
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private GameObject _previousMenu;
        [SerializeField] private Toggle _toggleFullscreen;
        [SerializeField] private Toggle _toggleInvertYAxis;
        [SerializeField] private Toggle _toggleInvertXAxis;
        [SerializeField] private Slider _sliderSensitivity;
        [SerializeField] private Slider _sliderMusic;
        [SerializeField] private Slider _sliderSounds;
        private string _fullscreenKey = Config.DISPLAY_FULLSCREEN;
        private string _invertYAxisKey = Config.CAMERA_INVERT_Y_AXIS;
        private string _invertXAxisKey = Config.CAMERA_INVERT_X_AXIS;
        private string _sensitivityKey = Config.CAMERA_SENSITIVITY;
        private string _musicKey = Config.VOLUME_MUSIC;
        private string _soundsKey = Config.VOLUME_SOUNDS;

        private void Awake()
        {
            // Initialize values
            _toggleFullscreen.isOn = Screen.fullScreen;
            _toggleInvertYAxis.isOn = Config.current.invertYAxis;
            _toggleInvertXAxis.isOn = Config.current.invertXAxis;
            _sliderSensitivity.value = Config.current.cameraSensitivity;
            if (PlayerPrefs.HasKey(Config.VOLUME_MUSIC))
            {
                _sliderMusic.value = PlayerPrefs.GetFloat(Config.VOLUME_MUSIC);
            }
            if (PlayerPrefs.HasKey(Config.VOLUME_SOUNDS))
            {
                _sliderSounds.value = PlayerPrefs.GetFloat(Config.VOLUME_SOUNDS);
            }
        }

        public void OnFullscreenChange()
        {
            Screen.fullScreen = _toggleFullscreen.isOn;
            PlayerPrefs.SetInt(_fullscreenKey, _toggleFullscreen.isOn ? 1 : 0);
        }

        public void OnInvertYAxisChange()
        {
            Config.current.invertYAxis = _toggleInvertYAxis.isOn;
            PlayerPrefs.SetInt(_invertYAxisKey, _toggleInvertYAxis.isOn ? 1 : 0);
            EventManager.Dispatch(EventNames.UpdateCameraSettings, null);
        }

        public void OnInvertXAxisChange()
        {
            Config.current.invertXAxis = _toggleInvertXAxis.isOn;
            PlayerPrefs.SetInt(_invertXAxisKey, _toggleInvertXAxis.isOn ? 1 : 0);
            EventManager.Dispatch(EventNames.UpdateCameraSettings, null);
        }

        public void OnSensitivityChange()
        {
            Config.current.cameraSensitivity = _sliderSensitivity.value;
            PlayerPrefs.SetFloat(_sensitivityKey, _sliderSensitivity.value);
            EventManager.Dispatch(EventNames.UpdateCameraSettings, null);
        }

        public void OnMusicVolumeChange()
        {
            PlayerPrefs.SetFloat(_musicKey, _sliderMusic.value);
            AkSoundEngine.SetRTPCValue(_musicKey, PlayerPrefs.GetFloat(Config.VOLUME_MUSIC));
        }

        public void OnSoundsVolumeChange()
        {
            PlayerPrefs.SetFloat(_soundsKey, _sliderSounds.value);
            AkSoundEngine.SetRTPCValue(_soundsKey, PlayerPrefs.GetFloat(Config.VOLUME_SOUNDS));
        }

        public void BtnBack()
        {
            gameObject.SetActive(false);
            _previousMenu.SetActive(true);
        }
    }
}
