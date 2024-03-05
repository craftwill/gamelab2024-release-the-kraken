
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public abstract class BaseEntityController : MonoBehaviourPun
    {
        [SerializeField] protected float _moveSpeed = 2.5f;
        [SerializeField] protected float _attackRange = 2.5f;

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
    }
}
