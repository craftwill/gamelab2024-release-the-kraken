using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class ComboComponent : MonoBehaviour
    {
        [SerializeField] LayerMask _enemyLayer;
        bool _hitAnEnemy = false;

        private void OnEnable()
        {
            _hitAnEnemy = false;
        }

        private void OnDisable()
        {
            if (_hitAnEnemy)
            {
                EventManager.Dispatch(EventNames.IncreaseCombo, null);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _enemyLayer) != 0)
            {
                _hitAnEnemy = true;
            }
        }
    }
}
