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

        protected override void Awake()
        {
            base.Awake();

            _healthComponent.MaxHealth = _config.maxHealth;
            _attackComponent.InitSettings(_config.damageDealt, _config.attackCooldown, _config.attackDuration, _config.lockedIntoAttackDuration);
            _entityController.InitSettings(_config.moveSpeed, _config.attackRange);
        }

        protected override void HandleTakeDamage(float dmgAmount)
        {
            base.HandleTakeDamage(dmgAmount);

            _entityAnimationComponent.PlayHurtAnim();
        }

        protected override void HandleDie()
        {
            base.HandleDie();

            photonView.RPC(nameof(RPC_All_Die), RpcTarget.All);
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

            Animate.Delay(1.5f, () =>
            {
                if (this == null) return;
                PhotonNetwork.Destroy(photonView);
            }, true);
        }

        // Disable controller.
        public void DisableControllerAndEnablePhysics() 
        {
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<SphereCollider>().isTrigger = false;
            Rigidbody rg = GetComponent<Rigidbody>();
            rg.isKinematic = false;
        }

        public void EnableControllerAndDisablePhysics()
        {
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<SphereCollider>().isTrigger = true;
            Rigidbody rg = GetComponent<Rigidbody>();
            rg.isKinematic = true;
        }
    }
}
