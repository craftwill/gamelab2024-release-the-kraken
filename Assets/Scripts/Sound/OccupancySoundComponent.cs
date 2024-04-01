using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class OccupancySoundComponent : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event _fullCapacitySound;

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

        public void PlayFullCapacitySound()
        {
            _fullCapacitySound.Post(gameObject);
        }
    }
}
