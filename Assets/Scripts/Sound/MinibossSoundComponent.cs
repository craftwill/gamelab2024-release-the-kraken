using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MinibossSoundComponent : MonoBehaviour
    {
        [SerializeField] AK.Wwise.Event _StompSound;

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
        
        public void PlayStompSound()
        {
            _StompSound.Post(gameObject);
        }
    }
}