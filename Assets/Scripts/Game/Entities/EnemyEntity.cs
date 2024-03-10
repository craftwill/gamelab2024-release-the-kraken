using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

using Bytes;
using Kraken.Network;

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
        [SerializeField] private GameObject _woolPrefab;
        [SerializeField] private int _woolDropped = 1;

        protected override void Awake()
        {
            base.Awake();

            _healthComponent.MaxHealth = _config.maxHealth;
            if (_attackComponent) 
                _attackComponent.InitSettings(_config.damageDealt, _config.attackCooldown, _config.attackDuration, _config.lockedIntoAttackDuration, _config.rangedProjectile);
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
                photonView.RPC(nameof(RPC_All_SpawnWool), RpcTarget.All);
            }
        }

        // send flying enemy with physics when it dies
        [PunRPC]
        private void RPC_All_Die() 
        {
            GetComponent<PhotonTransformView>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<SphereCollider>().isTrigger = false;
            Rigidbody rg = GetComponent<Rigidbody>();
            rg.isKinematic = false;

            Vector3 closestPlayerPos = GetClosestPlayer().Item1.transform.position;
            Vector3 dirToSend = -(closestPlayerPos - this.transform.position).normalized;
            Vector3 verticalForce = new Vector3(0f, Random.Range(1f, 10f), 0f);
            rg.AddForce(dirToSend * 35f + verticalForce, ForceMode.Impulse);
            rg.AddTorque(new Vector3(Random.Range(3f, 8f), Random.Range(3f, 8f), Random.Range(3f, 8f)), ForceMode.Impulse);

            if (!PhotonNetwork.IsMasterClient) return;

            Animate.Delay(2.5f, () => 
            {
                if (this == null) return;
                PhotonNetwork.Destroy(photonView);
            }, true);
        }

        public void DisableControllerAndEnablePhysics()
        {
            _entityController.SetControllerActive(false);
            GetComponent<SphereCollider>().isTrigger = false;
            Rigidbody rg = GetComponent<Rigidbody>();
            rg.isKinematic = false;
        }

        public void EnableControllerAndDisablePhysics()
        {
            _entityController.SetControllerActive(true);
            GetComponent<SphereCollider>().isTrigger = true;
            Rigidbody rg = GetComponent<Rigidbody>();
            rg.isKinematic = true;
        }

        public Kraken.Game.HealthComponent GetHealthComponent()
        {
            return _healthComponent;
        }

        [PunRPC]
        private void RPC_All_SpawnWool()
        {
            for (int i = 0; i < _woolDropped; i++)
            {
                Instantiate(_woolPrefab, gameObject.transform.position, Quaternion.identity);
            }
        }
    }
}
