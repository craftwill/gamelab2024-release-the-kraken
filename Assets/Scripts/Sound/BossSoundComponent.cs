using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class BossSoundComponent : MonoBehaviourPun
    {
        [SerializeField] private AK.Wwise.Event _bossSpawnSound;
        [SerializeField] private AK.Wwise.Event _starfallHitSound;
        [SerializeField] private AK.Wwise.Event _ringsOfLightHitSound;
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

        public void PlayBossSpawnSound()
        {
            _bossSpawnSound.Post(gameObject);
        }

        public void PlayStarfallHitSound(GameObject position = null)
        {   
            if (position != null)
            {
                _starfallHitSound.Post(position);
            }
            else
            {
                _starfallHitSound.Post(gameObject);
            }
        }

        public void PlayRingsOfLightHitSound(GameObject position = null)
        {
            if (position != null)
            {
                _starfallHitSound.Post(position);
            }
            else
            {
                _starfallHitSound.Post(gameObject);
            }
        }
    }

}
