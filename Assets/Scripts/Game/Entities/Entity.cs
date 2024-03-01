using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Kraken.Game;

namespace Kraken
{
    public enum EntityClan
    {
        Ally,
        Hostile
    }

    public abstract class Entity : MonoBehaviourPun
    {
        [SerializeField] protected EntityAnimationComponent _entityAnimationComponent;
        [SerializeField] protected HealthComponent _healthComponent;

        [field: SerializeField] public EntityClan EntityClan { get; private set; } = EntityClan.Hostile;

        protected PlayerEntity[] _playerEntitiesList;

        protected virtual void Awake()
        {
            _healthComponent.OnTakeDamage.AddListener(HandleTakeDamage);
            _healthComponent.OnDie.AddListener(HandleDie);

            _playerEntitiesList = FindObjectsOfType<PlayerEntity>();
        }

        protected virtual void HandleTakeDamage(float dmgAmount)
        {

        }

        protected virtual void HandleDie()
        {

        }

        protected virtual void OnDestroy()
        {
            _healthComponent.OnTakeDamage.RemoveAllListeners();
            _healthComponent.OnDie.RemoveAllListeners();
        }

        public (PlayerEntity, float) GetClosestPlayer()
        {
            PlayerEntity closestPlayer = null;
            float closestDistance = 9999;
            // Get Closest Target
            foreach (var playerEntity in _playerEntitiesList)
            {
                if (playerEntity == null) { continue; }

                float distance = Vector3.Distance(playerEntity.transform.position, this.transform.position);
                if (distance <= closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = playerEntity;
                }
            }

            return (closestPlayer, closestDistance);
        }
    }
}
