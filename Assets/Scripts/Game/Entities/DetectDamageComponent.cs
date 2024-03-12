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
                var inflictDmgComp = other.GetComponent<InflictDamageComponent>();
                if(inflictDmgComp != null)
                {
                    ProcessDamage(inflictDmgComp);
                }
            }
        }

        private void ProcessDamage(InflictDamageComponent inflictDmgComp) 
        {
            bool canDamage = _takeDamageFromClans.Contains(inflictDmgComp.Damageclan);

            if (!canDamage) return;

            // If its a projectile, call ProjectileHit on it.
            BaseProjectile projectile = inflictDmgComp.gameObject.GetComponent<BaseProjectile>();
            if (projectile != null)
            {
                canDamage = canDamage && projectile.CanDamage;
                projectile.ProjectileHit();
            }

            if (canDamage)
            {
                OnDetectDamage.Invoke(inflictDmgComp.Damage);
            }
        }

        public void TakeDamageFromOtherSource(float damage)
        {
            OnDetectDamage.Invoke(damage);
        }
    }
}
