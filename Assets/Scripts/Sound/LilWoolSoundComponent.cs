using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class LilWoolSoundComponent : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event _pickupSound;
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

        public void PlayPickupSound()
        {
            _pickupSound.Post(gameObject);
        }
    }
}
