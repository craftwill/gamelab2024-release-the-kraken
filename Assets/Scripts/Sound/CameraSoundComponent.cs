using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class CameraSoundComponent : MonoBehaviour
    {
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
    }
}
