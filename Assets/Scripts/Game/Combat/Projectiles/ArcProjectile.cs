using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class ArcProjectile : BaseProjectile
    {
        [SerializeField] private float _arcStrength = 0.15f;
        private Vector3 _customVelocity;

        public override void InitAndSend(Vector3 direction, EntityClan damageClan, float damage)
        {
            base.InitAndSend(direction, damageClan, damage);

            _customVelocity = new Vector3(0, _arcStrength, 0);
        }

        private void FixedUpdate()
        {
            transform.position += _currentDirection * _speed + _customVelocity;
            _customVelocity.y -= 0.01f;
        }
    }
}
