using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kraken.Game
{
    public class DetectDamageComponent : MonoBehaviour
    {
        public UnityEvent<float> OnDetectDamage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "DealDamage")
            {
                OnDetectDamage.Invoke(1);
            }
        }
    }
}
