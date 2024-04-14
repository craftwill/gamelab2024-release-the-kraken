using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MainMenuSoundComponent : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event _buttonHoverSound;
        [SerializeField] private AK.Wwise.Event _buttonClickSound;
        [SerializeField] private AK.Wwise.Event _sliderSound;

        private void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
        }

        private void OnDestroy()
        {
            AkSoundEngine.UnregisterGameObj(gameObject);
        }

        private void Update()
        {
            AkSoundEngine.SetObjectPosition(gameObject, transform);
        }

        public void PlayButtonHoverSound()
        {
            _buttonHoverSound.Post(gameObject);
        }

        public void PlayButtonClickSound()
        {
            _buttonClickSound.Post(gameObject);
        }

        public void PlaySliderSound()
        {
            _sliderSound.Post(gameObject);
        }
    }

}
