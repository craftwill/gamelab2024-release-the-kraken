
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;

using Bytes;
using Kraken.Network;
using System.Collections;

namespace Kraken
{
    public class EnemyEntity : Entity
    {
        [SerializeField] private EnemyConfigSO _config;
        [SerializeField] private EntityAttackComponent _attackComponent;
        [SerializeField] private BaseEntityController _entityController;
        [SerializeField] private EnemyZoneComponent _enemyZoneComponent;
        [SerializeField] private PathfindingEntityController _pathfindingEntityController;
        [SerializeField] private EnemySoundComponent _soundComponent;
        [SerializeField] private GameObject _minimapIcon;
        [SerializeField] private GameObject _woolPrefab;
        [SerializeField] private GameObject _smokePoofPrefab;
        [SerializeField] private int _woolDropped = 1;
        [SerializeField] LayerMask _zoneOccupancyLayer;

        private float _initialRigibodyDrag;

        protected override void Awake()
        {
            base.Awake();

            _healthComponent.MaxHealth = _config.maxHealth;
            if (_attackComponent) 
                _attackComponent.InitSettings(_config.damageDealt, _config.attackCooldown, _config.attackDuration, _config.lockedIntoAttackDuration, _config.rangedProjectile);
            _entityController.InitSettings(_config);
            _enemyZoneComponent.InitSettings(_config.zoneOccupancyCount);

            // Store to restore later if enemy drag was changed trough an ability or script and enemy dies
            _initialRigibodyDrag = GetComponent<Rigidbody>().drag;
        }

        protected virtual void Start()
        {
            EventManager.AddEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
        }

        protected override void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.StopGameFlow, HandleStopGameFlow);
            if (!gameObject.scene.isLoaded) return;
            Instantiate(_smokePoofPrefab, transform.position, Quaternion.identity);
        }

        public virtual void TakeUltimateDamage(float dmgAmount)
        {
            base.TakeDamage(dmgAmount);
        }

        protected virtual void HandleStopGameFlow(BytesData data) 
        {
            _entityController.enabled = false;
            _pathfindingEntityController.SetControllerActive(false);
        }

        protected override void HandleTakeDamage(float dmgAmount)
        {
            base.HandleTakeDamage(dmgAmount);

            if(_pathfindingEntityController is BasicEntityController)
            {
                _entityAnimationComponent.PlayHurtAnim();
                _pathfindingEntityController.Stagger();
            }

            photonView.RPC(nameof(_soundComponent.RPC_All_PlayEnemyHurtSound), RpcTarget.All);
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
                System.Array.ForEach(colliders, x => { if (((1 << x.gameObject.layer) & _zoneOccupancyLayer) != 0) x.enabled = false; });
            }
        }

        // send flying enemy with physics when it dies
        [PunRPC]
        protected void RPC_All_Die() 
        {
            //Spawn wool
            for (int i = 0; i < _woolDropped; i++)
            {
                Instantiate(_woolPrefab, gameObject.transform.position, Quaternion.identity);
            }

            if (_pathfindingEntityController is BasicEntityController)
            {
                GetComponent<PhotonTransformView>().enabled = false;
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<SphereCollider>().isTrigger = false;
                Rigidbody rg = GetComponent<Rigidbody>();
                rg.drag = _initialRigibodyDrag;
                rg.isKinematic = false;

                Vector3 closestPlayerPos = GetClosestPlayer().Item1.transform.position;
                Vector3 dirToSend = -(closestPlayerPos - this.transform.position).normalized;
                Vector3 verticalForce = new Vector3(0f, Random.Range(1f, 10f), 0f);
                rg.AddForce(dirToSend * 35f + verticalForce, ForceMode.Impulse);
                rg.AddTorque(new Vector3(Random.Range(3f, 8f), Random.Range(3f, 8f), Random.Range(3f, 8f)), ForceMode.Impulse);
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(photonView);
                return;
            }
            

            if (!PhotonNetwork.IsMasterClient) return;

            Animate.Delay(1.5f, () => 
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

        public PathfindingEntityController GetController()
        {
            return _pathfindingEntityController;
        }
    }
}
