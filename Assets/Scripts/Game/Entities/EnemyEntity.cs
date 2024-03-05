using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

using Bytes;

namespace Kraken
{
    public class EnemyEntity : Entity
    {
        [SerializeField] private EnemyConfigSO _config;
        [SerializeField] private EntityAttackComponent _attackComponent;
        [SerializeField] private BaseEntityController _entityController;
        [SerializeField] private EnemyZoneComponent _enemyZoneComponent;
        [SerializeField] private PathfindingEntityController _pathfindingEntityController;
        [SerializeField] private GameObject _minimapIcon;

        protected override void Awake()
        {
            base.Awake();

            _healthComponent.MaxHealth = _config.maxHealth;
            if (_attackComponent) _attackComponent.InitSettings(_config.damageDealt, _config.attackCooldown, _config.attackDuration, _config.lockedIntoAttackDuration);
            _entityController.InitSettings(_config);
            _enemyZoneComponent.InitSettings(_config.zoneOccupancyCount);
        }

        protected virtual void Start()
        {
            EventManager.AddEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        protected override void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        protected virtual void HandleStopGameFlow(BytesData data) 
        {
            _healthComponent.TakeDamage(1_000_000);
        }

        protected override void HandleTakeDamage(float dmgAmount)
        {
            base.HandleTakeDamage(dmgAmount);

            _entityAnimationComponent.PlayHurtAnim();
            _pathfindingEntityController.Stagger();
        }

        protected override void HandleDie()
        {
            base.HandleDie();

            _minimapIcon.SetActive(false);
            photonView.RPC(nameof(RPC_All_Die), RpcTarget.All);

            if (PhotonNetwork.IsMasterClient)
            {
                _enemyZoneComponent.RemoveEnemyFromZones();

                //remove colliders to not interfere with ontriggerexit
                var colliders = GetComponentsInChildren<Collider>();
                System.Array.ForEach(colliders, x => x.enabled = false);
            }
        }

        // send flying enemy with physics when it dies
        [PunRPC]
        private void RPC_All_Die() 
        {
            GetComponent<PhotonTransformView>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;

            SphereCollider colAdded = gameObject.AddComponent<SphereCollider>();
            colAdded.radius = 0.2f;

            Rigidbody rgAdded = gameObject.AddComponent<Rigidbody>();
            Vector3 closestPlayerPos = GetClosestPlayer().Item1.transform.position;
            Vector3 dirToSend = -(closestPlayerPos - this.transform.position).normalized;
            Vector3 verticalForce = new Vector3(0f, Random.Range(1f, 10f), 0f);
            rgAdded.AddForce(dirToSend * 35f + verticalForce, ForceMode.Impulse);
            rgAdded.AddTorque(new Vector3(Random.Range(3f, 8f), Random.Range(3f, 8f), Random.Range(3f, 8f)), ForceMode.Impulse);

            if (!PhotonNetwork.IsMasterClient) return;

            Animate.Delay(2.5f, () => 
            {
                if (this == null) return;
                PhotonNetwork.Destroy(photonView);
            }, true);
        }

        public Kraken.Game.HealthComponent GetHealthComponent()
        {
            return _healthComponent;
        }
    }
}
