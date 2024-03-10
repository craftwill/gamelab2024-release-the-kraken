
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public abstract class BaseEntityController : MonoBehaviourPun
    {
        protected float _moveSpeed = 2.5f;
        protected float _attackRange = 2.5f;
        protected bool _isActive = true;

        public virtual void InitSettings(EnemyConfigSO config)
        {
            _attackRange = config.attackRange;
            _moveSpeed = config.moveSpeed;
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        public virtual void SetControllerActive(bool isActive) 
        {
            _isActive = isActive;
        }
    }
}
