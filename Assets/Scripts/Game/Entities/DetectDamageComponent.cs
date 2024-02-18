using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Kraken.Game
{
    public class DetectDamageComponent : MonoBehaviour
    {
        public UnityEvent<float> OnDetectDamage;

        [SerializeField] private EntityClan[] _takeDamageFromClans;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "DealDamage")
            {
                var idc = other.GetComponent<InflictDamageComponent>();
                if(idc is not null)
                {
                    if (_takeDamageFromClans.Contains(idc.Damageclan))
                    {
                        OnDetectDamage.Invoke(idc.Damage);
                    }
                }
                else
                {
                    OnDetectDamage.Invoke(1);
                }
                
            }
        }
    }
}
