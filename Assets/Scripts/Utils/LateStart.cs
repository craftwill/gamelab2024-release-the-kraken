using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bytes;

namespace Kraken
{
    public class LateStart : MonoBehaviour
    {
        public const float LATE_START_DELAY = 0.25f;

        private void Start()
        {
            Animate.Delay(LATE_START_DELAY, dispatchLateStart);
        }

        private void dispatchLateStart() 
        {
            EventManager.Dispatch(EventNames.LateStart, null);
        }
    }
}
