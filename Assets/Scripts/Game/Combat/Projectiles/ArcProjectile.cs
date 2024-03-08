using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class ArcProjectile : BaseProjectile
    {
        private Vector3 _customVelocity = new Vector3(0, 0.17f, 0f);
        private void FixedUpdate()
        {
            transform.position += _currentDirection * _speed + _customVelocity;
            _customVelocity.y -= 0.01f;
        }
    }
}
