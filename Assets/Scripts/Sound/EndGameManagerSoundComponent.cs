using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kraken
{
    public class EndGameManagerSoundComponent : MonoBehaviourPun
    {
        [SerializeField] private AK.Wwise.Event _victorySound;
        [SerializeField] private AK.Wwise.Event _defeatSound;

        private void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
        }

        private void OnDestroy()
        {
            AkSoundEngine.UnregisterGameObj(gameObject);
        }

        public void PlayVictorySound()
        {
            _victorySound.Post(gameObject);
        }

        public void PlayDefeatSound()
        {
            _defeatSound.Post(gameObject);
        }
    }

}
