using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class BossSoundComponent : MonoBehaviourPun
    {
        [SerializeField] private AK.Wwise.Event _bossSpawnSound;
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
    }

}
