using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class TowerSoundComponent : MonoBehaviour
    {
        [SerializeField] AK.Wwise.Event _playTowerSound;
        [SerializeField] AK.Wwise.Event _stopTowerSound;

        private void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
            _playTowerSound.Post(gameObject);
        }

        private void OnDestroy()
        {
            _stopTowerSound.Post(gameObject);
            AkSoundEngine.UnregisterGameObj(gameObject);
        }

        private void Update()
        {
            AkSoundEngine.SetObjectPosition(gameObject, transform);
        }
    }

}
