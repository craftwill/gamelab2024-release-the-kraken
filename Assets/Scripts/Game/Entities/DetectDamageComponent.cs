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
                var idc = other.GetComponent<InflictDamageComponent>();
                if(idc is not null)
                {
                    OnDetectDamage.Invoke(idc.Damage);
                }
                else
                {
                    OnDetectDamage.Invoke(1);
                }
                
            }
        }
    }
}
