
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public abstract class BaseEntityController : MonoBehaviourPun
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] protected float _attackRange = 2.5f;
        protected bool _isActive = true;

        public virtual void InitSettings(EnemyConfigSO config)
        {
            _moveSpeed = config.moveSpeed;
            _attackRange = config.attackRange;
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
