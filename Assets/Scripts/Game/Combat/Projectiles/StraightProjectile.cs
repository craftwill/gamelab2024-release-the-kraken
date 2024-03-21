using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class StraightProjectile : BaseProjectile
    {
        private void FixedUpdate()
        {
            transform.position += _currentDirection * _speed;
        }
    }
}
