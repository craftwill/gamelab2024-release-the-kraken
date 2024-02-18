
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public abstract class BaseEntityController : MonoBehaviourPun
    {
        [SerializeField] protected float _moveSpeed = 1f;
        [SerializeField] protected float _attackRange = 2.5f;

        public virtual void InitSettings(float moveSpeed, float attackRange)
        {
            _moveSpeed = moveSpeed;
            _attackRange = attackRange;
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
