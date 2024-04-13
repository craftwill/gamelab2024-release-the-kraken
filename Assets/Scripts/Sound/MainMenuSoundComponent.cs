using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MainMenuSoundComponent : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event _buttonHoverSound;
        [SerializeField] private AK.Wwise.Event _buttonClickSound;
        [SerializeField] private AK.Wwise.Event _mainMenuMusic;
        [SerializeField] private AK.Wwise.Event _mainMenuMusicStop;

        public void PlayButtonHoverSound()
        {
            _buttonHoverSound.Post(gameObject);
        }

        public void PlayButtonClickSound()
        {
            _buttonClickSound.Post(gameObject);
        }

        public void PlayMenuMusic()
        {
            _mainMenuMusic.Post(gameObject);
        }

        public void StopMenuMusic()
        {
            _mainMenuMusicStop.Post(gameObject);
        }
    }

}
