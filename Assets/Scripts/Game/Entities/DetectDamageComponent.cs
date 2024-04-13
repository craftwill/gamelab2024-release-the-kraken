using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Kraken.Game
{
    public class DetectDamageComponent : MonoBehaviour
    {
        public UnityEvent<float> OnDetectDamage;

        [SerializeField] private EntityClan[] _takeDamageFromClans;
        [SerializeField] private bool _getKnockedBack = false;
        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private HealthComponent _healthComponent;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "DealDamage")
            {
                var inflictDmgComp = other.GetComponent<InflictDamageComponent>();
                if(inflictDmgComp != null)
                {
                    ProcessDamage(inflictDmgComp);
                    if (_getKnockedBack)
                    {
                        Vector3 direction = Vector3.zero;
                        if (inflictDmgComp.Source == null)
                        {
                            direction = (transform.position - inflictDmgComp.transform.position).normalized;
                        }
                        else
                        {
                            direction = (transform.position - inflictDmgComp.Source.position).normalized;
                        }
                        StartCoroutine(KnockbackCoroutine(direction));
                    }
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

        private IEnumerator KnockbackCoroutine(Vector3 direction)
        {
            _rigidBody.isKinematic = false;
            if (_agent.isOnNavMesh)
                _agent.isStopped = true;
            _rigidBody.AddForce(direction * Config.current.knockbackForce, ForceMode.Impulse);
            yield return new WaitForSeconds(Config.current.knockBackDuration);
            if (_healthComponent.IsAlive)
            {
                _rigidBody.isKinematic = true;
                if (_agent.isOnNavMesh)
                    _agent.isStopped = false;
            }   
        }
    }
}
